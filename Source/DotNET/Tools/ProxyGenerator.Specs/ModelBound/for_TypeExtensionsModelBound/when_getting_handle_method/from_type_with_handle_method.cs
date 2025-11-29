// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound.when_getting_handle_method;

public class from_type_with_handle_method : Specification
{
    MethodInfo _result;

    void Because() => _result = typeof(ValidCommand).GetHandleMethod();

    [Fact] void should_return_handle_method() => _result.Name.ShouldEqual("Handle");
}
