// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Types;

namespace Cratis.Applications;

/// <summary>
/// Internal properties.
/// </summary>
internal static class Internals
{
    static IServiceProvider? _serviceProvider;
    static ITypes? _types;

    /// <summary>
    /// Internal: The service provider.
    /// </summary>
    /// <exception cref="ServiceProviderNotConfigured">Thrown if the service provider has not been configured.</exception>
    internal static IServiceProvider ServiceProvider
    {
        get
        {
            if (_serviceProvider == null)
            {
                throw new ServiceProviderNotConfigured();
            }

            return _serviceProvider;
        }
        set => _serviceProvider = value;
    }

    /// <summary>
    /// Internal: The types.
    /// </summary>
    /// <exception cref="TypeDiscoverySystemNotConfigured">Thrown if the type discovery system has not been configured.</exception>
    internal static ITypes Types
    {
        get
        {
            if (_types == null)
            {
                throw new TypeDiscoverySystemNotConfigured();
            }

            return _types;
        }
        set => _types = value;
    }
}
