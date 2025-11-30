// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http.Json;
using System.Text.Json;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Bridges the JavaScript runtime with an HTTP client for testing proxies end-to-end.
/// Intercepts fetch calls from JavaScript and routes them to the test HTTP client.
/// </summary>
public sealed class JavaScriptHttpBridge : IDisposable
{
    readonly JavaScriptRuntime _runtime;
    readonly HttpClient _httpClient;
    readonly JsonSerializerOptions _jsonOptions;
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="JavaScriptHttpBridge"/> class.
    /// </summary>
    /// <param name="runtime">The JavaScript runtime.</param>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    public JavaScriptHttpBridge(JavaScriptRuntime runtime, HttpClient httpClient)
    {
        _runtime = runtime;
        _httpClient = httpClient;
        _jsonOptions = Json.Globals.JsonSerializerOptions;
        SetupFetchInterceptor();
    }

    /// <summary>
    /// Gets the JavaScript runtime.
    /// </summary>
    public JavaScriptRuntime Runtime => _runtime;

    /// <summary>
    /// Gets the HTTP client.
    /// </summary>
    public HttpClient HttpClient => _httpClient;

    /// <summary>
    /// Loads TypeScript code into the runtime by transpiling it first.
    /// </summary>
    /// <param name="typeScriptCode">The TypeScript code to load.</param>
    public void LoadTypeScript(string typeScriptCode)
    {
        var jsCode = _runtime.TranspileTypeScript(typeScriptCode);
        _runtime.Execute(jsCode);
    }

    /// <summary>
    /// Executes a command through its JavaScript proxy class.
    /// The proxy's execute() method will call fetch(), which is intercepted and routed to HTTP.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="commandClassName">The name of the command class.</param>
    /// <param name="properties">The property values to set on the command.</param>
    /// <returns>The command execution result.</returns>
    /// <exception cref="JavaScriptProxyExecutionFailed">The exception that is thrown when the proxy execution fails.</exception>
    public async Task<CommandExecutionResult<TResult>> ExecuteCommandViaProxyAsync<TResult>(
        string commandClassName,
        Dictionary<string, object> properties)
    {
        // Set up properties on the command
        var propAssignments = string.Concat(properties.Select(p =>
            $"__cmd.{p.Key} = {JsonSerializer.Serialize(p.Value, _jsonOptions)};"));

        // Create command instance and set properties, then call execute()
        _runtime.Execute(
            "var __cmd = new " + commandClassName + "();" +
            propAssignments +
            "var __cmdResult = null;" +
            "var __cmdError = null;" +
            "var __cmdDone = false;" +
            "__cmd.execute().then(function(result) {" +
            "    __cmdResult = result;" +
            "    __cmdDone = true;" +
            "}).catch(function(error) {" +
            "    __cmdError = error;" +
            "    __cmdDone = true;" +
            "});");

        // Process the pending fetch request
        var result = await ProcessPendingFetchAsync();

        // Wait for promise resolution
        SpinWait.SpinUntil(() => (bool)_runtime.Evaluate("__cmdDone")!, TimeSpan.FromSeconds(5));

        var hasError = _runtime.Evaluate<bool>("__cmdError !== null");
        if (hasError)
        {
            var errorMsg = _runtime.Evaluate<string>("__cmdError?.message || String(__cmdError)");
            throw new JavaScriptProxyExecutionFailed($"Command execution failed: {errorMsg}");
        }

        // Get the result from JavaScript
        var resultJson = _runtime.Evaluate<string>("JSON.stringify(__cmdResult)") ?? "{}";
        var commandResult = JsonSerializer.Deserialize<Arc.Commands.CommandResult<TResult>>(resultJson, _jsonOptions);

        return new CommandExecutionResult<TResult>(commandResult, result.ResponseJson);
    }

    /// <summary>
    /// Performs a query through its JavaScript proxy class.
    /// The proxy's perform() method will call fetch(), which is intercepted and routed to HTTP.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="queryClassName">The name of the query class.</param>
    /// <param name="parameters">Optional parameter values for the query.</param>
    /// <returns>The query execution result.</returns>
    /// <exception cref="JavaScriptProxyExecutionFailed">The exception that is thrown when the proxy execution fails.</exception>
    public async Task<QueryExecutionResult<TResult>> PerformQueryViaProxyAsync<TResult>(
        string queryClassName,
        Dictionary<string, object>? parameters = null)
    {
        // Set up parameters on the query
        var paramAssignments = parameters is not null
            ? string.Concat(parameters.Select(p => $"__query.{p.Key} = {JsonSerializer.Serialize(p.Value, _jsonOptions)};"))
            : string.Empty;

        // Create query instance and set parameters, then call perform()
        _runtime.Execute(
            "var __query = new " + queryClassName + "();" +
            paramAssignments +
            "var __queryResult = null;" +
            "var __queryError = null;" +
            "var __queryDone = false;" +
            "__query.perform().then(function(result) {" +
            "    __queryResult = result;" +
            "    __queryDone = true;" +
            "}).catch(function(error) {" +
            "    __queryError = error;" +
            "    __queryDone = true;" +
            "});");

        // Process the pending fetch request
        var result = await ProcessPendingFetchAsync();

        // Wait for promise resolution
        SpinWait.SpinUntil(() => (bool)_runtime.Evaluate("__queryDone")!, TimeSpan.FromSeconds(5));

        var hasError = _runtime.Evaluate<bool>("__queryError !== null");
        if (hasError)
        {
            var errorMsg = _runtime.Evaluate<string>("__queryError?.message || String(__queryError)");
            throw new JavaScriptProxyExecutionFailed($"Query execution failed: {errorMsg}");
        }

        // Get the result from JavaScript
        var resultJson = _runtime.Evaluate<string>("JSON.stringify(__queryResult)") ?? "{}";
        var queryResult = JsonSerializer.Deserialize<Arc.Queries.QueryResult>(resultJson, _jsonOptions);

        return new QueryExecutionResult<TResult>(queryResult, result.ResponseJson);
    }

    /// <summary>
    /// Executes a command directly via HTTP without going through JavaScript proxy.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="route">The route to POST to.</param>
    /// <param name="payload">The command payload.</param>
    /// <returns>The command result.</returns>
    public async Task<CommandExecutionResult<TResult>> ExecuteCommandDirectAsync<TResult>(string route, object payload)
    {
        var response = await _httpClient.PostAsJsonAsync(route, payload, _jsonOptions);
        var responseContent = await response.Content.ReadAsStringAsync();
        var commandResult = JsonSerializer.Deserialize<Arc.Commands.CommandResult<TResult>>(responseContent, _jsonOptions);
        return new CommandExecutionResult<TResult>(commandResult, responseContent);
    }

    /// <summary>
    /// Executes a command directly via HTTP without going through JavaScript proxy.
    /// </summary>
    /// <param name="route">The route to POST to.</param>
    /// <param name="payload">The command payload.</param>
    /// <returns>The command result.</returns>
    public async Task<CommandExecutionResult<object>> ExecuteCommandDirectAsync(string route, object payload)
    {
        var response = await _httpClient.PostAsJsonAsync(route, payload, _jsonOptions);
        var responseContent = await response.Content.ReadAsStringAsync();
        var commandResult = JsonSerializer.Deserialize<Arc.Commands.CommandResult<object>>(responseContent, _jsonOptions);
        return new CommandExecutionResult<object>(commandResult, responseContent);
    }

    /// <summary>
    /// Performs a query directly via HTTP without going through JavaScript proxy.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="route">The route to GET from.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The query result.</returns>
    public async Task<QueryExecutionResult<TResult>> PerformQueryDirectAsync<TResult>(string route, Dictionary<string, object>? parameters = null)
    {
        var fullRoute = route;
        if (parameters?.Count > 0)
        {
            var queryString = string.Join('&', parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value?.ToString() ?? string.Empty)}"));
            fullRoute = route.Contains('?') ? $"{route}&{queryString}" : $"{route}?{queryString}";
        }

        var response = await _httpClient.GetAsync(fullRoute);
        var responseContent = await response.Content.ReadAsStringAsync();
        var queryResult = JsonSerializer.Deserialize<Arc.Queries.QueryResult>(responseContent, _jsonOptions);
        return new QueryExecutionResult<TResult>(queryResult, responseContent);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _runtime.Dispose();
            _disposed = true;
        }
    }

    void SetupFetchInterceptor()
    {
        // Set up the fetch interceptor that stores pending requests
        _runtime.Execute(
            "var __pendingFetch = null;" +
            "function fetch(url, options) {" +
            "    return new Promise(function(resolve, reject) {" +
            "        __pendingFetch = {" +
            "            url: url," +
            "            options: options || {}," +
            "            resolve: resolve," +
            "            reject: reject" +
            "        };" +
            "    });" +
            "}");
    }

    async Task<FetchResult> ProcessPendingFetchAsync()
    {
        // Get the pending fetch request details
        var hasPending = _runtime.Evaluate<bool>("__pendingFetch !== null");
        if (!hasPending)
        {
            throw new InvalidOperationException("No pending fetch request found");
        }

        var url = _runtime.Evaluate<string>("__pendingFetch.url") ?? string.Empty;
        var method = _runtime.Evaluate<string>("__pendingFetch.options.method || 'GET'") ?? "GET";
        var bodyJson = _runtime.Evaluate<string>("__pendingFetch.options.body || null");

        // Make the actual HTTP request
        HttpResponseMessage response;
        if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var content = new StringContent(bodyJson ?? "{}", System.Text.Encoding.UTF8, "application/json");
            response = await _httpClient.PostAsync(url, content);
        }
        else
        {
            response = await _httpClient.GetAsync(url);
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        // Resolve the JavaScript promise with the response
        var escapedResponse = responseContent
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");

        _runtime.Execute(
            "if (__pendingFetch) {" +
            "    var responseData = JSON.parse('" + escapedResponse + "');" +
            "    __pendingFetch.resolve({" +
            "        ok: " + (response.IsSuccessStatusCode ? "true" : "false") + "," +
            "        status: " + (int)response.StatusCode + "," +
            "        json: function() { return Promise.resolve(responseData); }," +
            "        text: function() { return Promise.resolve('" + escapedResponse + "'); }" +
            "    });" +
            "    __pendingFetch = null;" +
            "}");

        return new FetchResult(url, method, responseContent);
    }

    record FetchResult(string Url, string Method, string ResponseJson);
}

/// <summary>
/// The exception that is thrown when a JavaScript proxy execution fails.
/// </summary>
/// <param name="message">The error message.</param>
public class JavaScriptProxyExecutionFailed(string message) : Exception(message);

/// <summary>
/// Represents the result of executing a command through the JavaScript proxy.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
/// <param name="Result">The command result.</param>
/// <param name="RawJson">The raw JSON response.</param>
public record CommandExecutionResult<TResult>(Arc.Commands.CommandResult<TResult>? Result, string RawJson);

/// <summary>
/// Represents the result of performing a query through the JavaScript proxy.
/// </summary>
/// <typeparam name="TResult">The type of the data.</typeparam>
/// <param name="Result">The query result.</param>
/// <param name="RawJson">The raw JSON response.</param>
public record QueryExecutionResult<TResult>(Arc.Queries.QueryResult? Result, string RawJson);
