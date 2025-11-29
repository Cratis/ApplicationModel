// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound.when_getting_query_methods;

public class from_type_without_query_methods : Specification
{
    IEnumerable<MethodInfo> _result;

    void Because() => _result = typeof(ReadModelWithoutQueryMethods).GetQueryMethods();

    [Fact] void should_return_empty_collection() => _result.ShouldBeEmpty();
}
