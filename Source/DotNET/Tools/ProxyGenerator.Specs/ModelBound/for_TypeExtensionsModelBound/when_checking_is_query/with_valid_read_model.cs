// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound.when_checking_is_query;

public class with_valid_read_model : Specification
{
    bool _result;

    void Because() => _result = typeof(ValidReadModel).IsQuery();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
