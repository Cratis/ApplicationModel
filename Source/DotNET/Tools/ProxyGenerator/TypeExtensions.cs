// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Cratis.Applications.ProxyGenerator.Templates;
using Microsoft.Extensions.DependencyModel;

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Extension methods for working with types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the definition of any type.
    /// </summary>
    public static readonly TargetType AnyType = new("any", "Object");

    /// <summary>
    /// Gets the definition of any type that is a final one.
    /// </summary>
    public static readonly TargetType AnyTypeFinal = new("any", "Object", Final: true);

#pragma warning disable SA1600 // Elements should be documented
    internal static Type _conceptType = typeof(object);
    internal static Type _nullableType = typeof(Nullable<>);
    internal static Type _expandoObjectType = typeof(ExpandoObject);
    internal static Type _stringType = typeof(string);
    internal static Type _enumerableType = typeof(IEnumerable);
    internal static Type _genericEnumerableType = typeof(IEnumerable<>);
    internal static Type _dictionaryType = typeof(IDictionary<,>);
    internal static Type _asyncEnumerableType = typeof(IAsyncEnumerable<>);
    internal static Type _controllerBaseType = typeof(object);
    internal static Type _taskType = typeof(Task);
    internal static Type _voidType = typeof(void);
#pragma warning restore SA1600 // Elements should be documented

    static readonly Dictionary<string, TargetType> _primitiveTypeMap = new()
    {
        { typeof(object).FullName!, AnyTypeFinal },
        { typeof(char).FullName!, new("string", "String") },
        { typeof(byte).FullName!, new("number", "Number") },
        { typeof(sbyte).FullName!, new("number", "Number") },
        { typeof(bool).FullName!, new("boolean", "Boolean") },
        { typeof(string).FullName!, new("string", "String") },
        { typeof(short).FullName!, new("number", "Number") },
        { typeof(int).FullName!, new("number", "Number") },
        { typeof(long).FullName!, new("number", "Number") },
        { typeof(ushort).FullName!, new("number", "Number") },
        { typeof(uint).FullName!, new("number", "Number") },
        { typeof(ulong).FullName!, new("number", "Number") },
        { typeof(float).FullName!, new("number", "Number") },
        { typeof(double).FullName!, new("number", "Number") },
        { typeof(decimal).FullName!, new("number", "Number") },
        { typeof(DateTime).FullName!, new("Date",  "Date") },
        { typeof(DateTimeOffset).FullName!, new("Date", "Date") },
        { typeof(Guid).FullName!, new("Guid", "Guid", "@cratis/fundamentals") },
        { typeof(DateOnly).FullName!, new("Date", "Date") },
        { typeof(TimeOnly).FullName!, new("Date", "Date") },
        { typeof(System.Text.Json.Nodes.JsonNode).FullName!, AnyTypeFinal },
        { typeof(System.Text.Json.Nodes.JsonObject).FullName!, AnyTypeFinal },
        { typeof(System.Text.Json.Nodes.JsonArray).FullName!, AnyTypeFinal },
        { typeof(System.Text.Json.JsonDocument).FullName!, AnyTypeFinal },
        { typeof(Uri).FullName!, new("string", "String") }
    };

    static readonly Dictionary<string, Assembly> _assembliesByName = [];

    static PathAssemblyResolver? _assemblyResolver;
    static MetadataLoadContext? _metadataLoadContext;

    /// <summary>
    /// Gets all assemblies gathered from the <see cref="InitializeProjectAssemblies(string, Action{string}, Action{string})"/> method.
    /// </summary>
    public static IEnumerable<Assembly> Assemblies { get; private set; } = [];

    /// <summary>
    /// Initialize the project assemblies.
    /// </summary>
    /// <param name="assemblyFile">Assembly file to start from.</param>
    /// <param name="message">Callback for outputting messages.</param>
    /// <param name="errorMessage">Callback for outputting error messages.</param>
    /// <returns>True if successful, false if not.</returns>
    public static bool InitializeProjectAssemblies(string assemblyFile, Action<string> message, Action<string> errorMessage)
    {
        message($"  Gather all project referenced assemblies for {assemblyFile}");
        var assemblyFolder = Path.GetDirectoryName(assemblyFile)!;

        var assembly = Assembly.LoadFile(assemblyFile);
        var dependencyContext = DependencyContext.Load(assembly);
        if (dependencyContext is null)
        {
            errorMessage($"Could not load dependency context for assembly '{assemblyFile}'");
            return false;
        }

        var root = RuntimeEnvironment.GetRuntimeDirectory();
        root = Path.GetDirectoryName(root)!;
        var version = Path.GetFileName(root)!;
        var framework = Directory.GetParent(root)!;
        var shared = Directory.GetParent(framework.FullName)!;
        var aspNetCoreAppPath = Path.Combine(shared.FullName, "Microsoft.AspNetCore.App", version);

        message($"  Runtime assemblies: {root}");
        message($"  AspNetCoreApp assemblies: {aspNetCoreAppPath}");

        var runtimeAssemblies = Directory.GetFiles(root, "*.dll");
        var aspNetCoreAssemblies = Directory.GetFiles(aspNetCoreAppPath, "*.dll");
        var appAssemblies = Directory.GetFiles(assemblyFolder, "*.dll");
        string[] paths = [.. runtimeAssemblies, .. aspNetCoreAssemblies, .. appAssemblies];

        _assemblyResolver = new PathAssemblyResolver(paths);
#pragma warning disable CA2000 // Dispose objects before losing scope
        _metadataLoadContext = new MetadataLoadContext(_assemblyResolver);
#pragma warning restore CA2000 // Dispose objects before losing scope

        AssemblyLoadContext.Default.Resolving += (_, name) =>
            _assembliesByName[name.Name!] = _assemblyResolver.Resolve(_metadataLoadContext, name)!;

        Assemblies = dependencyContext.RuntimeLibraries
                                        .Where(_ => _.Type.Equals("project"))
                                        .Select(_ => _metadataLoadContext.LoadFromAssemblyPath(Path.Join(assemblyFolder, $"{_.Name}.dll")))
                                        .Where(_ => _ is not null)
                                        .Distinct()
                                        .ToArray();

        foreach (var loadedAssembly in _metadataLoadContext.GetAssemblies())
        {
            _assembliesByName[loadedAssembly.GetName().Name!] = loadedAssembly;
        }

        InitializeWellKnownTypes();

        return true;
    }

    /// <summary>
    /// Check if a type is a controller.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if it is a controller, false if not.</returns>
    public static bool IsController(this Type type) => type.IsAssignableTo(_controllerBaseType);

    /// <summary>
    /// Check if a type is an async enumerable.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if it is an async enumerable, false if not.</returns>
    public static bool IsAsyncEnumerable(this Type type) => type.IsAssignableTo(_asyncEnumerableType);

    /// <summary>
    /// Check if a type is observable.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if it is observable, false if not.</returns>
    public static bool IsSubject(this Type type) => type.FullName!.StartsWith("System.Reactive.Subjects.ISubject`1");

    /// <summary>
    /// Check if a type is a String or not.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if type is a string, false otherwise.</returns>
    public static bool IsString(this Type type) => type == _stringType;

    /// <summary>
    /// Check if a type is assignable to a specific type.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <typeparam name="T">Type to check if is assignable to.</typeparam>
    /// <returns>True if it is, false if not.</returns>
    public static bool IsAssignableTo<T>(this Type type)
    {
        if (_assembliesByName.TryGetValue(typeof(T).Assembly.GetName().Name!, out var assembly))
        {
            return type.IsAssignableTo(assembly.GetType(typeof(T).FullName!));
        }
        return false;
    }

    /// <summary>
    /// Check whether or not a <see cref="Type"/> is a known type in TypeScript.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if it is known, false if not.</returns>
    public static bool IsKnownType(this Type type)
    {
        if (type.IsDictionary())
        {
            return true;
        }

        if (type.IsConcept())
        {
            type = type.GetConceptValueType();
        }

        return _primitiveTypeMap.ContainsKey(type.FullName!);
    }

    /// <summary>
    /// Check if a type is a dictionary.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if it is, false if not.</returns>
    public static bool IsDictionary(this Type type) =>
        (type.IsGenericType && type.GetGenericTypeDefinition() == _dictionaryType) ||
        type.GetInterfaces().Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == _dictionaryType);

    /// <summary>
    /// Get property descriptors for a type.
    /// </summary>
    /// <param name="type">Type to get for.</param>
    /// <returns>Collection of <see cref="PropertyDescriptor"/>.</returns>
    public static IEnumerable<PropertyDescriptor> GetPropertyDescriptors(this Type type)
    {
        if (type.IsAPrimitiveType()) return [];

        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList().ConvertAll(_ => _.ToPropertyDescriptor());
    }

    /// <summary>
    /// Get target type for a type.
    /// </summary>
    /// <param name="type">Type to get for.</param>
    /// <returns>The <see cref="TargetType"/>.</returns>
    public static TargetType GetTargetType(this Type type)
    {
        if (type.IsDictionary())
        {
            return AnyTypeFinal;
        }

        if (type.IsConcept())
        {
            type = type.GetConceptValueType();
        }

        if (_primitiveTypeMap.TryGetValue(type.FullName!, out var value))
        {
            return value;
        }

        return new TargetType(type.Name, type.Name);
    }

    /// <summary>
    /// Convert a <see cref="Type"/> to a <see cref="TypeDescriptor"/>.
    /// </summary>
    /// <param name="type">Type to convert.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <returns>Converted <see cref="TypeDescriptor"/>.</returns>
    public static TypeDescriptor ToTypeDescriptor(this Type type, string targetPath, int segmentsToSkip)
    {
        var typesInvolved = new List<Type>();

        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        var propertyDescriptors = properties.ConvertAll(_ => _.ToPropertyDescriptor());
        List<ImportStatement> imports = [];

        foreach (var property in propertyDescriptors)
        {
            if (!property.OriginalType.IsKnownType())
            {
                property.CollectTypesInvolved(typesInvolved);
            }
            if (!string.IsNullOrEmpty(property.Module))
            {
                imports.Add(new ImportStatement(property.Type, property.Module));
            }
        }
        imports.AddRange(typesInvolved.GetImports(targetPath, type!.ResolveTargetPath(segmentsToSkip), segmentsToSkip));

        return new TypeDescriptor(
            type,
            type.GetTargetType().Type,
            propertyDescriptors,
            imports,
            typesInvolved);
    }

    /// <summary>
    /// Convert a <see cref="Type"/> to a <see cref="ModelDescriptor"/>.
    /// </summary>
    /// <param name="type">Type to convert.</param>
    /// <returns>Converted <see cref="ModelDescriptor"/>.</returns>
    public static ModelDescriptor ToModelDescriptor(this Type type)
    {
        var isSubject = type.IsSubject();
        if (isSubject)
        {
            type = type.GetSubjectElementType()!;
        }
        var isAsyncEnumerable = type.IsAsyncEnumerable();
        if (isAsyncEnumerable)
        {
            type = type.GetAsyncEnumerableElementType()!;
        }

        var isEnumerable = type.IsEnumerable();
        if (isEnumerable)
        {
            type = type.GetEnumerableElementType()!;
        }

        var targetType = type.GetTargetType();

        return new(
            type,
            targetType.Type,
            targetType.Constructor,
            isEnumerable,
            isSubject || isAsyncEnumerable,
            []);
    }

    /// <summary>
    /// Convert a <see cref="Type"/> to a <see cref="EnumDescriptor"/>.
    /// </summary>
    /// <param name="type">Enum type to convert.</param>
    /// <returns>Converted <see cref="EnumDescriptor"/>.</returns>
    public static EnumDescriptor ToEnumDescriptor(this Type type)
    {
        var enumUnderlyingType = Enum.GetUnderlyingType(type);
        var values = Enum.GetValues(type).Cast<object>().Select(value => Convert.ChangeType(value, enumUnderlyingType)).ToArray();
        var names = Enum.GetNames(type);
        var members = values.Select((value, index) => new EnumMemberDescriptor(names[index], value)).ToArray();
        return new EnumDescriptor(type, type.Name, members, []);
    }

    /// <summary>
    /// Resolve the relative path for a type.
    /// </summary>
    /// <param name="type">Type to resolve for.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <returns>Resolved path.</returns>
    public static string ResolveTargetPath(this Type type, int segmentsToSkip)
    {
        var ns = type.Namespace!.Replace(Globals.NamespacePrefix, string.Empty);
        var path = string.Join(Path.DirectorySeparatorChar, ns.Split('.').Skip(segmentsToSkip));
        if (string.IsNullOrEmpty(path))
        {
            path = $"{Path.DirectorySeparatorChar}";
        }

        return path;
    }

    /// <summary>
    /// Get imports from a collection of types.
    /// </summary>
    /// <param name="types">Types to get from.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="relativePath">The relative path to work from.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <returns>A collection of <see cref="ImportStatement"/>.</returns>
    public static IEnumerable<ImportStatement> GetImports(this IEnumerable<Type> types, string targetPath, string relativePath, int segmentsToSkip) =>
         types.Select(_ =>
        {
            var fullPath = Path.Join(targetPath, relativePath);
            var fullPathForType = Path.Join(targetPath, _.ResolveTargetPath(segmentsToSkip));
            var importPath = Path.GetRelativePath(fullPath, fullPathForType);
            importPath = $"{importPath}/{_.Name}";
            return new ImportStatement(_.GetTargetType().Type, importPath);
        }).ToArray();

    /// <summary>
    /// Collect types involved for a property, recursively.
    /// </summary>
    /// <param name="property">Property to collect for.</param>
    /// <param name="typesInvolved">Collected types involved.</param>
    /// <remarks>It skips any types already added to the collection passed to it.</remarks>
    public static void CollectTypesInvolved(this PropertyDescriptor property, IList<Type> typesInvolved)
    {
        if (typesInvolved.Contains(property.OriginalType) || property.OriginalType.IsAPrimitiveType() || property.OriginalType.IsConcept()) return;
        typesInvolved.Add(property.OriginalType);
        foreach (var subProperty in property.OriginalType.GetPropertyDescriptors().Where(_ => !_.OriginalType.IsKnownType()))
        {
            CollectTypesInvolved(subProperty, typesInvolved);
        }
    }

    /// <summary>
    /// Collect types involved for an argument, recursively.
    /// </summary>
    /// <param name="argument">Argument to collect for.</param>
    /// <param name="typesInvolved">Collected types involved.</param>
    /// <remarks>It skips any types already added to the collection passed to it.</remarks>
    public static void CollectTypesInvolved(this RequestArgumentDescriptor argument, IList<Type> typesInvolved)
    {
        if (typesInvolved.Contains(argument.OriginalType)) return;
        typesInvolved.Add(argument.OriginalType);
        foreach (var subProperty in argument.OriginalType.GetPropertyDescriptors().Where(_ => !_.OriginalType.IsKnownType()))
        {
            CollectTypesInvolved(subProperty, typesInvolved);
        }
    }

    /// <summary>
    /// Check if a type is enumerable. Note that string is an IEnumerable, but in this case the string is excluded, as well as ExpandoObject.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if type is enumerable, false if not an enumerable.</returns>
    public static bool IsEnumerable(this Type type)
    {
        return !type.IsAPrimitiveType() && type != _expandoObjectType && !type.IsString() && _enumerableType.IsAssignableFrom(type);
    }

    /// <summary>
    /// Check if a type is a "primitive" type.  This is not just dot net primitives but basic types like string, decimal, datetime,
    /// that are not classified as primitive types.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if <see cref="Type"/> is a primitive type.</returns>
    public static bool IsAPrimitiveType(this Type type)
    {
        return type.GetTypeInfo().IsPrimitive
                || type.IsNullable() || _primitiveTypeMap.ContainsKey(type.FullName!);
    }

    /// <summary>
    /// Check if a type is nullable or not.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>True if type is nullable, false if not.</returns>
    public static bool IsNullable(this Type type)
    {
        while (!type.Equals(typeof(object)))
        {
            if (type.GetTypeInfo().IsGenericType &&
               type.GetGenericTypeDefinition() == _nullableType)
            {
                return true;
            }

            if (type.BaseType is null) break;

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Gets the element type of an enumerable.
    /// </summary>
    /// <param name="enumerableType">The <see cref="Type">type of the enumerable</see>.</param>
    /// <returns>Enumerable element <see cref="Type"/>.</returns>
    /// <remarks>
    /// https://stackoverflow.com/questions/906499/getting-type-t-from-ienumerablet.
    /// </remarks>
    public static Type GetEnumerableElementType(this Type enumerableType)
    {
        if (enumerableType.IsArray)
        {
            return enumerableType.GetElementType()!;
        }

        if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == _genericEnumerableType)
        {
            return enumerableType.GetGenericArguments()[0];
        }

        return enumerableType.GetInterfaces()
            .Where(t => t.IsGenericType &&
                t.GetGenericTypeDefinition() == _genericEnumerableType)
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault()!;
    }

    /// <summary>
    /// Gets the element type of a System.Reactive.Subjects.ISubject{T}.
    /// </summary>
    /// <param name="subjectType">The <see cref="Type"/> to get from.</param>
    /// <returns>The element type.</returns>
    public static Type GetSubjectElementType(this Type subjectType)
    {
        if (subjectType.IsGenericType && subjectType.IsSubject() )
        {
            return subjectType.GetGenericArguments()[0];
        }

        return subjectType.GetInterfaces()
            .Where(t => t.IsGenericType &&
                t.GetGenericTypeDefinition().Name.StartsWith("ISubject`1"))
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault()!;
    }

    /// <summary>
    /// Gets the element type of an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <param name="asyncEnumerableType">The <see cref="Type"/> to get from.</param>
    /// <returns>The element type.</returns>
    public static Type GetAsyncEnumerableElementType(this Type asyncEnumerableType)
    {
        if (asyncEnumerableType.IsGenericType && asyncEnumerableType.GetGenericTypeDefinition() == _asyncEnumerableType)
        {
            return asyncEnumerableType.GetGenericArguments()[0];
        }

        return asyncEnumerableType.GetInterfaces()
            .Where(t => t.IsGenericType &&
                t.GetGenericTypeDefinition() == _asyncEnumerableType)
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault()!;
    }

    /// <summary>
    /// Check if a type derives from an open generic type.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <param name="openGenericType">Open generic <see cref="Type"/> to check for.</param>
    /// <returns>True if type matches the open generic <see cref="Type"/>.</returns>
    public static bool IsDerivedFromOpenGeneric(this Type type, Type openGenericType)
    {
        var typeToCheck = type;
        while (typeToCheck != null && typeToCheck != typeof(object))
        {
            var currentType = typeToCheck.GetTypeInfo().IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
            if (openGenericType == currentType)
                return true;

            typeToCheck = typeToCheck.GetTypeInfo().BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if a type is a concept or not.
    /// </summary>
    /// <param name="objectType"><see cref="Type"/> to check.</param>
    /// <returns>True if type is a concept, false if not.</returns>
    public static bool IsConcept(this Type objectType)
    {
        return objectType.IsDerivedFromOpenGeneric(_conceptType);
    }

    /// <summary>
    /// Get the type of the value inside a concept.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to get value type from.</param>
    /// <returns>The type of the concept value.</returns>
    public static Type GetConceptValueType(this Type type)
    {
        return ConceptMap.GetConceptValueType(type);
    }

    static void InitializeWellKnownTypes()
    {
        var assembly = _metadataLoadContext!.CoreAssembly!;
        _nullableType = assembly.GetType(typeof(Nullable<>).FullName!)!;
        _expandoObjectType = assembly.GetType(typeof(ExpandoObject).FullName!)!;
        _stringType = assembly.GetType(typeof(string).FullName!)!;
        _enumerableType = assembly.GetType(typeof(IEnumerable).FullName!)!;
        _genericEnumerableType = assembly.GetType(typeof(IEnumerable<>).FullName!)!;
        _asyncEnumerableType = assembly.GetType(typeof(IAsyncEnumerable<>).FullName!)!;
        _dictionaryType = assembly.GetType(typeof(IDictionary<,>).FullName!)!;
        _taskType = assembly.GetType(typeof(Task).FullName!)!;
        _voidType = assembly.GetType(typeof(void).FullName!)!;

        var fundamentals = _metadataLoadContext.LoadFromAssemblyName("Cratis.Fundamentals")!;
        _conceptType = fundamentals.GetType("Cratis.Concepts.ConceptAs`1")!;

        var aspNetCore = _metadataLoadContext.LoadFromAssemblyName("Microsoft.AspNetCore.Mvc.Core");
        _controllerBaseType = aspNetCore.GetType("Microsoft.AspNetCore.Mvc.ControllerBase")!;
    }
}
