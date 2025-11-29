// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound.when_checking_has_query_methods;

public class with_type_having_query_methods : Specification
{
    bool _result;

    void Because() => _result = typeof(ValidReadModel).HasQueryMethods();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
