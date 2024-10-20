// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text.Json.Nodes;
using Cratis.Applications.Orleans.Concepts;
using Cratis.Applications.Swagger;
using Cratis.Execution;
using Cratis.Json;
using Orleans.Serialization;
using Serilog;
using Serilog.Exceptions;

// Force invariant culture for the Backend
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetails()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
builder.UseCratisApplicationModel();
builder.UseCratisMongoDB();

// Todo: This should be part of the "Use application model" extension method, with overrides
builder.Services
    .AddMicrosoftIdentityPlatformIdentityAuthentication()
    .AddSwaggerGen(options =>
    {
        var files = Directory.GetFiles(AppContext.BaseDirectory).Where(file => Path.GetExtension(file) == ".xml");
        var documentationFiles = files.Where(file =>
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var dllFileName = Path.Combine(AppContext.BaseDirectory, $"{fileName}.dll");
                var xmlFileName = Path.Combine(AppContext.BaseDirectory, $"{fileName}.xml");
                return File.Exists(dllFileName) && File.Exists(xmlFileName);
            });

        foreach (var file in documentationFiles)
        {
            options.IncludeXmlComments(file);
        }
        options.AddConcepts();
    })
    .AddEndpointsApiExplorer()
    .AddMvc();

builder.UseOrleans(_ => _
    .ConfigureServices(services =>
    {
        services.AddConceptSerializer();
        services.AddSerializer(serializerBuilder => serializerBuilder.AddJsonSerializer(
            _ =>
                _ == typeof(JsonObject) ||
                (_.Namespace?.StartsWith("Read") ?? false) ||
                (_.Namespace?.StartsWith("Concepts") ?? false),
            Globals.JsonSerializerOptions));
    })
    .UseLocalhostClustering()
    .AddCratisMongoDBStorageAsDefault()
    .UseDashboard(options =>
    {
        options.Host = "*";
        options.Port = 8081;
        options.HostSelf = true;
    }));

var app = builder.Build();
app.UseCratisApplicationModel();
app.UseDefaultFiles();
app.UseStaticFiles();

if (RuntimeEnvironment.IsDevelopment)
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger"));
}

app.UseWebSockets();
app.MapControllers();
app.MapIdentityProvider();
app.UseAuthentication();
app.UseAuthorization();
app.MapFallbackToFile("/index.html");

await app.RunAsync();
