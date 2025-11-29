// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound;

[Command]
public class ValidCommand
{
    public string Name { get; set; } = string.Empty;

    public Task Handle() => Task.CompletedTask;
}

[Command]
public class CommandWithoutHandleMethod
{
    public string Name { get; set; } = string.Empty;
}

[Command]
public abstract class AbstractCommand
{
    public Task Handle() => Task.CompletedTask;
}

public class NonCommandClass
{
    public Task Handle() => Task.CompletedTask;
}
