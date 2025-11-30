// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// Provides file paths for JavaScript resources used in testing.
/// </summary>
public static class JavaScriptResources
{
    static readonly string _scenariosRoot;
    static readonly string _repoRoot;

    static JavaScriptResources()
    {
        // Find the Scenarios folder by looking for package.json
        var current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            var packageJson = Path.Combine(current, "package.json");
            if (File.Exists(packageJson) && current.EndsWith("Scenarios", StringComparison.OrdinalIgnoreCase))
            {
                _scenariosRoot = current;
                break;
            }

            current = Path.GetDirectoryName(current);
        }

        // If not found, calculate from assembly location (development scenario)
        if (string.IsNullOrEmpty(_scenariosRoot))
        {
            // From bin/Debug/net10.0 go up to ProxyGenerator.Specs then into Scenarios
            var assemblyDir = Path.GetDirectoryName(typeof(JavaScriptResources).Assembly.Location)!;
            _scenariosRoot = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "Scenarios"));
        }

        // Find repository root (has global.json)
        current = _scenariosRoot;
        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "global.json")))
            {
                _repoRoot = current;
                break;
            }

            current = Path.GetDirectoryName(current);
        }

        _repoRoot ??= Path.GetFullPath(Path.Combine(_scenariosRoot, "..", "..", "..", "..", "..", ".."));
    }

    /// <summary>
    /// Gets the path to the Scenarios folder.
    /// </summary>
    public static string ScenariosRoot => _scenariosRoot;

    /// <summary>
    /// Gets the path to the repository root.
    /// </summary>
    public static string RepoRoot => _repoRoot;

    /// <summary>
    /// Gets the path to the TypeScript compiler.
    /// </summary>
    public static string TypeScriptCompilerPath =>
        Path.Combine(_repoRoot, "node_modules", "typescript", "lib", "typescript.js");

    /// <summary>
    /// Gets the path to the Arc package CJS directory.
    /// </summary>
    public static string ArcPackagePath =>
        Path.Combine(_repoRoot, "Source", "JavaScript", "Arc", "dist", "cjs");

    /// <summary>
    /// Gets the path to the Arc.React package CJS directory.
    /// </summary>
    public static string ArcReactPackagePath =>
        Path.Combine(_repoRoot, "Source", "JavaScript", "Arc.React", "dist", "cjs");

    /// <summary>
    /// Gets the path to the Fundamentals package CJS directory.
    /// </summary>
    public static string FundamentalsPackagePath =>
        Path.Combine(_repoRoot, "node_modules", "@cratis", "fundamentals", "dist", "cjs");

    /// <summary>
    /// Reads the TypeScript compiler source.
    /// </summary>
    /// <returns>The TypeScript compiler JavaScript code.</returns>
    /// <exception cref="TypeScriptCompilerNotFound">The exception that is thrown when the TypeScript compiler is not found.</exception>
    public static string GetTypeScriptCompiler()
    {
        var path = TypeScriptCompilerPath;
        if (!File.Exists(path))
        {
            throw new TypeScriptCompilerNotFound(path);
        }

        return File.ReadAllText(path);
    }

    /// <summary>
    /// Gets the Arc runtime bootstrap code that sets up the module environment.
    /// </summary>
    /// <returns>JavaScript code to bootstrap Arc modules.</returns>
    public static string GetArcBootstrap()
    {
        return GetBootstrapCode();
    }

    static string GetBootstrapCode()
    {
        return @"
// Arc Runtime Bootstrap for ClearScript V8
// This sets up a CommonJS-like module environment for testing generated proxies

// Node.js process shim (required by TypeScript compiler)
var process = {
    env: {},
    platform: 'darwin',
    version: 'v20.0.0',
    versions: { node: '20.0.0' },
    cwd: function() { return '/'; },
    nextTick: function(fn) { setTimeout(fn, 0); },
    stderr: { write: function() {} },
    stdout: { write: function() {} }
};

var __modules = {};
var __moduleCache = {};

// Simple require implementation
function require(modulePath) {
    if (__moduleCache[modulePath]) {
        return __moduleCache[modulePath].exports;
    }

    // Handle Arc package imports
    if (modulePath === '@cratis/arc' || modulePath.startsWith('@cratis/arc/')) {
        var subPath = modulePath === '@cratis/arc' ? 'index' : modulePath.replace('@cratis/arc/', '');
        return __loadArcModule(subPath);
    }

    // Handle Fundamentals imports
    if (modulePath === '@cratis/fundamentals' || modulePath.startsWith('@cratis/fundamentals/')) {
        var subPath = modulePath === '@cratis/fundamentals' ? 'index' : modulePath.replace('@cratis/fundamentals/', '');
        return __loadFundamentalsModule(subPath);
    }

    throw new Error('Module not found: ' + modulePath);
}

function __loadArcModule(subPath) {
    var key = '@cratis/arc/' + subPath;
    if (__moduleCache[key]) return __moduleCache[key].exports;

    var module = { exports: {} };
    __moduleCache[key] = module;

    // Arc module exports will be loaded by the host
    return module.exports;
}

function __loadFundamentalsModule(subPath) {
    var key = '@cratis/fundamentals/' + subPath;
    if (__moduleCache[key]) return __moduleCache[key].exports;

    var module = { exports: {} };
    __moduleCache[key] = module;

    return module.exports;
}

// Globals for Arc
var Globals = {
    microservice: '',
    apiBasePath: '',
    microserviceHttpHeader: 'X-Cratis-Microservice'
};

// Mock fetch that will be intercepted by the test bridge
var __fetchHandler = null;

function fetch(url, options) {
    return new Promise(function(resolve, reject) {
        if (__fetchHandler) {
            __fetchHandler(url, options, resolve, reject);
        } else {
            reject(new Error('No fetch handler configured'));
        }
    });
}

// AbortController mock
var AbortController = function() {
    this.signal = {};
};
AbortController.prototype.abort = function() {};

// URLSearchParams
var URLSearchParams = function(init) {
    this._params = {};
    if (init) {
        var pairs = init.toString().split('&');
        for (var i = 0; i < pairs.length; i++) {
            var pair = pairs[i].split('=');
            if (pair[0]) this._params[pair[0]] = pair[1] || '';
        }
    }
};
URLSearchParams.prototype.append = function(key, value) {
    this._params[key] = value;
};
URLSearchParams.prototype.toString = function() {
    var parts = [];
    for (var key in this._params) {
        parts.push(encodeURIComponent(key) + '=' + encodeURIComponent(this._params[key]));
    }
    return parts.join('&');
};
URLSearchParams.prototype.get = function(key) {
    return this._params[key];
};

// Headers mock
var Headers = function(init) {
    this._headers = {};
    if (init) {
        for (var key in init) {
            this._headers[key.toLowerCase()] = init[key];
        }
    }
};
Headers.prototype.append = function(key, value) {
    this._headers[key.toLowerCase()] = value;
};
Headers.prototype.get = function(key) {
    return this._headers[key.toLowerCase()];
};
";
    }
}

/// <summary>
/// The exception that is thrown when the TypeScript compiler is not found.
/// </summary>
/// <param name="path">The path where the compiler was expected.</param>
public class TypeScriptCompilerNotFound(string path)
    : Exception($"TypeScript compiler not found at '{path}'. Ensure you have run 'yarn install' in the repository root.");
