// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ProxyGenerator;

Console.WriteLine("Cratis Proxy Generator\n");

if (args.Length < 2)
{
    Console.WriteLine("Usage: ");
    Console.WriteLine("  Cratis.ProxyGenerator <assembly> <output-path> [segments-to-skip] [--skip-output-deletion] [--skip-type-name-in-route] [--api-prefix=<prefix>]");
    return 1;
}
var assemblyFile = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
var segmentsToSkip = args.Length > 2 ? int.Parse(args[2]) : 0;
var skipOutputDeletion = args.Any(_ => _ == "--skip-output-deletion");
var skipTypeNameInRoute = args.Any(_ => _ == "--skip-type-name-in-route");
var apiPrefixArg = args.FirstOrDefault(_ => _.StartsWith("--api-prefix="));
var apiPrefix = apiPrefixArg is null ? "api" : apiPrefixArg.Split('=')[^1];

var result = await Generator.Generate(
    assemblyFile,
    outputPath,
    segmentsToSkip,
    Console.WriteLine,
    Console.Error.WriteLine,
    skipOutputDeletion,
    skipTypeNameInRoute,
    apiPrefix);
return result ? 0 : 1;