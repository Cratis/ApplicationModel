// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http.Json;
using System.Text.Json;

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Bridges the JavaScript runtime with an HTTP client for testing proxies end-to-end.
/// Also provides direct HTTP execution for simpler end-to-end testing.
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
        SetupBridge();
    }

    /// <summary>
    /// Gets the JavaScript runtime.
    /// </summary>
    public JavaScriptRuntime Runtime { get; }

    /// <summary>
    /// Gets the HTTP client.
    /// </summary>
    public HttpClient HttpClient { get; }

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
    /// Executes a command directly via HTTP without JavaScript.
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
    /// Executes a command directly via HTTP without JavaScript.
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
    /// Performs a query directly via HTTP without JavaScript.
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

    /// <summary>
    /// Executes a command and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="commandName">The name of the command class.</param>
    /// <param name="properties">The property values to set on the command.</param>
    /// <returns>The command result.</returns>
    public async Task<CommandExecutionResult<TResult>> ExecuteCommandAsync<TResult>(string commandName, Dictionary<string, object> properties)
    {
        // Build the command instance in JavaScript
        var propAssignments = string.Join('\n', properties.Select(p => $"cmd.{p.Key} = {JsonSerializer.Serialize(p.Value, _jsonOptions)};"));
        var script = "var cmd = new " + commandName + "();" +
            propAssignments +
            "var __cmdPayload = {};" +
            "cmd.properties.forEach(function(prop) { __cmdPayload[prop] = cmd[prop]; });" +
            "var __cmdRoute = cmd.route;" +
            "JSON.stringify({ route: __cmdRoute, payload: __cmdPayload });";

        var resultJson = _runtime.Evaluate<string>(script);
        var commandInfo = JsonSerializer.Deserialize<CommandInfo>(resultJson, _jsonOptions);

        // Execute the HTTP request
        var response = await _httpClient.PostAsJsonAsync(commandInfo.Route, commandInfo.Payload, _jsonOptions);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Parse the response
        var commandResult = JsonSerializer.Deserialize<Arc.Commands.CommandResult<TResult>>(responseContent, _jsonOptions);

        return new CommandExecutionResult<TResult>(commandResult, responseContent);
    }

    /// <summary>
    /// Performs a query and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="queryName">The name of the query class.</param>
    /// <param name="parameters">The parameter values for the query.</param>
    /// <returns>The query result.</returns>
    public async Task<QueryExecutionResult<TResult>> PerformQueryAsync<TResult>(string queryName, Dictionary<string, object>? parameters = null)
    {
        // Build the query instance in JavaScript
        var paramAssignments = parameters is not null
            ? string.Join('\n', parameters.Select(p => $"query.{p.Key} = {JsonSerializer.Serialize(p.Value, _jsonOptions)};"))
            : string.Empty;

        var script = "var query = new " + queryName + "();" +
            paramAssignments +
            "var __queryRoute = query.route;" +
            "var __queryParams = {};" +
            "if (query.parameterDescriptors) {" +
            "    query.parameterDescriptors.forEach(function(desc) {" +
            "        if (query[desc.name] !== undefined) {" +
            "            __queryParams[desc.name] = query[desc.name];" +
            "        }" +
            "    });" +
            "}" +
            "JSON.stringify({ route: __queryRoute, parameters: __queryParams });";

        var resultJson = _runtime.Evaluate<string>(script);
        var queryInfo = JsonSerializer.Deserialize<QueryInfo>(resultJson, _jsonOptions);

        // Build query string
        var route = queryInfo.Route;
        if (queryInfo.Parameters?.Count > 0)
        {
            var queryString = string.Join('&', queryInfo.Parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value?.ToString() ?? string.Empty)}"));
            route = route.Contains('?') ? $"{route}&{queryString}" : $"{route}?{queryString}";
        }

        // Execute the HTTP request
        var response = await _httpClient.GetAsync(route);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Parse the response
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

    void SetupBridge()
    {
        // Set up origin to match the test server
        _runtime.Execute($"Globals.apiBasePath = '{_httpClient.BaseAddress}';");
    }

    record CommandInfo(string Route, Dictionary<string, object> Payload);
    record QueryInfo(string Route, Dictionary<string, object>? Parameters);
}

/// <summary>
/// Represents the result of executing a command through the JavaScript proxy.
/// </summary>
/// <typeparam name="TResult">The type of the response data.</typeparam>
/// <param name="Result">The command result.</param>
/// <param name="RawJson">The raw JSON response.</param>
public record CommandExecutionResult<TResult>(Arc.Commands.CommandResult<TResult> Result, string RawJson);

/// <summary>
/// Represents the result of performing a query through the JavaScript proxy.
/// </summary>
/// <typeparam name="TResult">The type of the data.</typeparam>
/// <param name="Result">The query result.</param>
/// <param name="RawJson">The raw JSON response.</param>
public record QueryExecutionResult<TResult>(Arc.Queries.QueryResult Result, string RawJson);
