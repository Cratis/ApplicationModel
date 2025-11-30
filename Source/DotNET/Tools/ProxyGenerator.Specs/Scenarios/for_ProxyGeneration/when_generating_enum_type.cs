// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Scenarios.for_Queries.ModelBound;
using Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_ProxyGeneration;

public class when_generating_enum_type : Specification, IDisposable
{
    JavaScriptRuntime _runtime = null!;
    string _generatedCode = null!;
    EnumDescriptor _descriptor = null!;
    bool _typeScriptIsValid;

    void Establish()
    {
        _runtime = new JavaScriptRuntime();

        var enumType = typeof(ReadModelStatus);
        var members = Enum.GetValues(enumType)
            .Cast<ReadModelStatus>()
            .Select(v => new EnumMemberDescriptor(v.ToString(), (int)v))
            .ToList();

        _descriptor = new EnumDescriptor(
            enumType,
            "ReadModelStatus",
            members,
            []);
    }

    void Because()
    {
        _generatedCode = InMemoryProxyGenerator.GenerateEnum(_descriptor);

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
    [Fact] void should_contain_enum_name() => _generatedCode.ShouldContain("ReadModelStatus");
    [Fact] void should_contain_unknown_member() => _generatedCode.ShouldContain("unknown");
    [Fact] void should_contain_active_member() => _generatedCode.ShouldContain("active");
    [Fact] void should_contain_inactive_member() => _generatedCode.ShouldContain("inactive");
    [Fact] void should_contain_archived_member() => _generatedCode.ShouldContain("archived");
    [Fact] void should_be_valid_typescript() => _typeScriptIsValid.ShouldBeTrue();

    public void Dispose()
    {
        _runtime?.Dispose();
        GC.SuppressFinalize(this);
    }
}
