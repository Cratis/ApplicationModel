// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Metrics;
using Cratis.Arc;
using Cratis.Conversion;
using Cratis.DependencyInjection;
using Cratis.Execution;
using Cratis.Serialization;
using Cratis.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Provides extension methods for <see cref="IHostBuilder"/> for configuring the application model services.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Gets the default section name for the application model configuration.
    /// </summary>
    public static readonly string[] DefaultArcSectionPaths = ["Cratis", "Arc"];

    /// <summary>
    /// Use Cratis Arc with the <see cref="IHostBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Binds the <see cref="ArcOptions"/> configuration to the given config section path or the default
    /// Cratis:Arc section path.
    /// </remarks>
    /// <param name="builder"><see cref="IHostBuilder"/> to extend.</param>
    /// <param name="configSectionPath">The optional configuration section path.</param>
    /// <returns><see cref="IHostBuilder"/> for building continuation.</returns>
    public static IHostBuilder UseCratisArc(this IHostBuilder builder, string? configSectionPath = null)
    {
        builder.ConfigureServices(_ => AddOptions(_)
                .BindConfiguration(configSectionPath ?? ConfigurationPath.Combine(DefaultArcSectionPaths)));

        return builder.UseArcImplementation();
    }

    /// <summary>
    /// Use Cratis Arc with the <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="IHostBuilder"/> to extend.</param>
    /// <param name="configureOptions">Action to configure the <see cref="ArcOptions"/>.</param>
    /// <returns><see cref="IHostBuilder"/> for building continuation.</returns>
    public static IHostBuilder UseCratisArc(this IHostBuilder builder, Action<ArcOptions> configureOptions)
    {
        builder.ConfigureServices(_ => AddOptions(_, configureOptions));
        var options = new ArcOptions();
        configureOptions(options);
        return builder.UseArcImplementation(options.IdentityDetailsProvider);
    }

    /// <summary>
    /// Add the Meter for the Application Model.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add the meter to.</param>
    /// <returns><see cref="IServiceCollection"/> for building continuation.</returns>
    public static IServiceCollection AddCratisArcMeter(this IServiceCollection services)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        services.TryAddKeyedSingleton(Internals.MeterName, new Meter(Internals.MeterName));
#pragma warning restore CA2000 // Dispose objects before losing scope
        return services;
    }

    static OptionsBuilder<ArcOptions> AddOptions(IServiceCollection services, Action<ArcOptions>? configureOptions = default)
    {
        var builder = services
            .AddOptions<ArcOptions>()
            .ValidateDataAnnotations()
            .ValidateOnStart();
        if (configureOptions is not null)
        {
            builder.Configure(configureOptions);
        }

        return builder;
    }

    static IHostBuilder UseArcImplementation(this IHostBuilder builder, Type? identityDetailsProvider = default)
    {
        Internals.Types = Types.Instance;
        Internals.Types.RegisterTypeConvertersForConcepts();
        TypeConverters.Register();
        var derivedTypes = DerivedTypes.Instance;

        builder.UseDefaultServiceProvider(_ => _.ValidateOnBuild = false);
        builder.AddCorrelationIdLogEnricher();

        builder
            .ConfigureServices(services =>
            {
                services.AddHttpContextAccessor();
                services.AddCratisArcMeter();
                services.AddCratisCommands();
                services.AddSingleton<ICorrelationIdAccessor>(sp => new CorrelationIdAccessor());
                services
                    .AddTypeDiscovery()
                    .AddSingleton<IDerivedTypes>(derivedTypes)
                    .AddControllersFromProjectReferencedAssembles(Internals.Types, derivedTypes)
                    .AddBindingsByConvention()
                    .AddSelfBindings();

                if (identityDetailsProvider is not null)
                {
                    services.AddIdentityProvider(identityDetailsProvider);
                }
                else
                {
                    services.AddIdentityProvider(Internals.Types);
                }
            });

        return builder;
    }
}
