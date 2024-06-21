// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications;
using Cratis.Conversion;
using Cratis.DependencyInjection;
using Cratis.Serialization;
using Cratis.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public static readonly string[] DefaultApplicationModelSectionPaths = ["Cratis", "ApplicationModel"];

    /// <summary>
    /// Use Cratis ApplicationModel with the <see cref="IHostBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Binds the <see cref="ApplicationModelOptions"/> configuration to the given config section path or the default
    /// Cratis:ApplicationModel section path.
    /// </remarks>
    /// <param name="builder"><see cref="IHostBuilder"/> to extend.</param>
    /// <param name="configSectionPath">The optional configuration section path.</param>
    /// <returns><see cref="IHostBuilder"/> for building continuation.</returns>
    public static IHostBuilder UseCratisApplicationModel(this IHostBuilder builder, string? configSectionPath = null)
    {
        builder.ConfigureServices(_ => AddOptions(_)
                .BindConfiguration(configSectionPath ?? ConfigurationPath.Combine(DefaultApplicationModelSectionPaths)));

        return builder.UseApplicationModelImplementation();
    }

    /// <summary>
    /// Use Cratis ApplicationModel with the <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="IHostBuilder"/> to extend.</param>
    /// <param name="configureOptions">Action to configure the <see cref="ApplicationModelOptions"/>.</param>
    /// <returns><see cref="IHostBuilder"/> for building continuation.</returns>
    public static IHostBuilder UseCratisApplicationModel(this IHostBuilder builder, Action<ApplicationModelOptions> configureOptions)
    {
        builder.ConfigureServices(_ => AddOptions(_, configureOptions));
        return builder.UseApplicationModelImplementation();
    }

    static OptionsBuilder<ApplicationModelOptions> AddOptions(IServiceCollection services, Action<ApplicationModelOptions>? configureOptions = default)
    {
        var builder = services
            .AddOptions<ApplicationModelOptions>()
            .ValidateDataAnnotations()
            .ValidateOnStart();
        if (configureOptions is not null)
        {
            builder.Configure(configureOptions);
        }

        return builder;
    }

    static IHostBuilder UseApplicationModelImplementation(this IHostBuilder builder)
    {
        Internals.Types = Types.Instance;
        Internals.Types.RegisterTypeConvertersForConcepts();
        TypeConverters.Register();
        var derivedTypes = DerivedTypes.Instance;

        builder.UseDefaultServiceProvider(_ => _.ValidateOnBuild = false);

        builder
            .ConfigureServices(_ => _
                .AddTypeDiscovery()
                .AddSingleton<IDerivedTypes>(derivedTypes)
                .AddIdentityProvider(Internals.Types)
                .AddControllersFromProjectReferencedAssembles(Internals.Types, derivedTypes)
                .AddBindingsByConvention()
                .AddSelfBindings());

        return builder;
    }
}
