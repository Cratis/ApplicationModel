// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_MethodInfoExtensions.when_checking_if_query_method;

public class with_http_post_method : Specification
{
    MethodInfo _method;
    bool _result;

    void Establish() => _method = typeof(TestController).GetMethod(nameof(TestController.Create))!;

    void Because() => _result = _method.IsQueryMethod();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
