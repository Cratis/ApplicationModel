// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;
using Cratis.Arc.ProxyGenerator.Scenarios.Queries;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_ProxyGeneration;

public class when_generating_type_with_nested_data : Specification, IDisposable
{
    JavaScriptRuntime _runtime = null!;
    string _generatedCode = null!;
    TypeDescriptor _descriptor = null!;
    bool _typeScriptIsValid;

    void Establish()
    {
        _runtime = new JavaScriptRuntime();

        var type = typeof(ComplexReadModel);
        var properties = type.GetProperties()
            .Select(p => p.ToPropertyDescriptor())
            .ToList();

        _descriptor = new TypeDescriptor(
            type,
            "ComplexReadModel",
            properties,
            Enumerable.Empty<ImportStatement>().OrderBy(_ => _.Module),
            []);
    }

    void Because()
    {
        _generatedCode = InMemoryProxyGenerator.GenerateType(_descriptor);

        try
        {
            var transpiledCode = _runtime.TranspileTypeScript(_generatedCode);
            _typeScriptIsValid = !string.IsNullOrEmpty(transpiledCode);
        }
        catch
        {
            _typeScriptIsValid = false;
        }
    }

    [Fact] void should_generate_code() => _generatedCode.ShouldNotBeEmpty();
    [Fact] void should_contain_class_name() => _generatedCode.ShouldContain("ComplexReadModel");
    [Fact] void should_contain_name_property() => _generatedCode.ShouldContain("name");
    [Fact] void should_contain_nested_data_property() => _generatedCode.ShouldContain("nestedData");
    [Fact] void should_contain_items_property() => _generatedCode.ShouldContain("items");
    [Fact] void should_contain_status_property() => _generatedCode.ShouldContain("status");
    [Fact] void should_be_valid_typescript() => _typeScriptIsValid.ShouldBeTrue();

    public void Dispose()
    {
        _runtime?.Dispose();
        GC.SuppressFinalize(this);
    }
}
