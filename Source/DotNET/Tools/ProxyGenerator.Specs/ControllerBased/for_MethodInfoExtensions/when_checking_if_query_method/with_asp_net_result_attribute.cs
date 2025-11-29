// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_MethodInfoExtensions.when_checking_if_query_method;

public class with_asp_net_result_attribute : Specification
{
    MethodInfo _method;
    bool _result;

    void Establish() => _method = typeof(TestController).GetMethod(nameof(TestController.GetAspNetResult))!;

    void Because() => _result = _method.IsQueryMethod();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
