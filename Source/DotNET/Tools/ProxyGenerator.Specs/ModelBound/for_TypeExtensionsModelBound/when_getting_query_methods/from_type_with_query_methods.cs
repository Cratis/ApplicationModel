// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound.when_getting_query_methods;

public class from_type_with_query_methods : Specification
{
    IEnumerable<MethodInfo> _result;

    void Because() => _result = typeof(ValidReadModel).GetQueryMethods();

    [Fact] void should_return_two_query_methods() => _result.Count().ShouldEqual(2);
    [Fact] void should_contain_get_by_id_method() => _result.Any(_ => _.Name == "GetById").ShouldBeTrue();
    [Fact] void should_contain_get_all_method() => _result.Any(_ => _.Name == "GetAll").ShouldBeTrue();
}
