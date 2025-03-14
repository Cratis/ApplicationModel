// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Represents the actual generator.
/// </summary>
public static class Generator
{
    /// <summary>
    /// Generate the proxies from an input assembly and output path.
    /// </summary>
    /// <param name="assemblyFile">Path to the assembly file.</param>
    /// <param name="outputPath">The output path to output to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <param name="message">Logger to use for outputting messages.</param>
    /// <param name="errorMessage">Logger to use for outputting error messages.</param>
    /// <returns>True if successful, false if not.</returns>
    public static async Task<bool> Generate(string assemblyFile, string outputPath, int segmentsToSkip, Action<string> message, Action<string> errorMessage)
    {
        assemblyFile = Path.GetFullPath(assemblyFile);
        if (!File.Exists(assemblyFile))
        {
            errorMessage($"Assembly file '{assemblyFile}' does not exist");
            return false;
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        var overallStopwatch = Stopwatch.StartNew();

        TypeExtensions.InitializeProjectAssemblies(assemblyFile, message, errorMessage);

        var commands = new List<MethodInfo>();
        var queries = new List<MethodInfo>();

        message($"  Discover controllers from {TypeExtensions.Assemblies.Count()} assemblies");

        foreach (var controller in TypeExtensions.Assemblies.SelectMany(_ => _.DefinedTypes).Where(__ => __.IsController()))
        {
            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            methods.Where(_ => _.IsQueryMethod()).ToList().ForEach(queries.Add);
            methods.Where(_ => _.IsCommandMethod()).ToList().ForEach(commands.Add);
        }

        message($"  Found {commands.Count} commands and {queries.Count} queries");

        if (Directory.Exists(outputPath)) Directory.Delete(outputPath, true);
        if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        var typesInvolved = new List<Type>();
        var directories = new List<string>();

        var commandDescriptors = commands.ConvertAll(_ => _.ToCommandDescriptor(outputPath, segmentsToSkip));
        await commandDescriptors.Write(outputPath, typesInvolved, TemplateTypes.Command, directories, segmentsToSkip, "commands", message);

        var queryDescriptors = queries.ConvertAll(_ => _.ToQueryDescriptor(outputPath, segmentsToSkip));

        var singleModelQueries = queryDescriptors.Where(_ => !_.IsEnumerable && !_.IsObservable).ToList();
        await singleModelQueries.Write(outputPath, typesInvolved, TemplateTypes.Query, directories, segmentsToSkip, "single model queries", message);

        var enumerableQueries = queryDescriptors.Where(_ => _.IsEnumerable).ToList();
        await enumerableQueries.Write(outputPath, typesInvolved, TemplateTypes.Query, directories, segmentsToSkip, "queries", message);

        var observableQueries = queryDescriptors.Where(_ => _.IsObservable).ToList();
        await observableQueries.Write(outputPath, typesInvolved, TemplateTypes.ObservableQuery, directories, segmentsToSkip, "observable queries", message);

        typesInvolved = [.. typesInvolved.Distinct()];
        var enums = typesInvolved.Where(_ => _.IsEnum).ToList();

        var typeDescriptors = typesInvolved.Where(_ => !enums.Contains(_)).ToList().ConvertAll(_ => _.ToTypeDescriptor(outputPath, segmentsToSkip));
        await typeDescriptors.Write(outputPath, typesInvolved, TemplateTypes.Type, directories, segmentsToSkip, "types", message);

        var enumDescriptors = enums.ConvertAll(_ => _.ToEnumDescriptor());
        await enumDescriptors.Write(outputPath, typesInvolved, TemplateTypes.Enum, directories, segmentsToSkip, "enums", message);

        var stopwatch = Stopwatch.StartNew();
        var directoriesWithContent = directories.Distinct().Select(_ => new DirectoryInfo(_));
        foreach (var directory in directoriesWithContent)
        {
            var exports = directory
                .GetFiles("*.ts")
                .Where(_ => _.Name != "index.ts")
                .Select(_ => $"./{Path.GetFileNameWithoutExtension(_.Name)}")
                .OrderBy(_ => _.Split('/')[^1]);
            var descriptor = new IndexDescriptor(exports);
            var content = TemplateTypes.Index(descriptor);
            await File.WriteAllTextAsync(Path.Join(directory.FullName, "index.ts"), content);
        }

        message($"  {directoriesWithContent.Count()} index files written in {stopwatch.Elapsed}");

        message($"  Overall time: {overallStopwatch.Elapsed}");

        return true;
    }
}
