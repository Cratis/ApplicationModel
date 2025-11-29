// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound;

[ReadModel]
public class ValidReadModel
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }

    public static Task<ValidReadModel> GetById(int id) => Task.FromResult(new ValidReadModel());
    public static Task<IEnumerable<ValidReadModel>> GetAll() => Task.FromResult<IEnumerable<ValidReadModel>>([]);
}

[ReadModel]
public class ReadModelWithoutQueryMethods
{
    public string Name { get; set; } = string.Empty;
}

[ReadModel]
public abstract class AbstractReadModel
{
    public static Task<AbstractReadModel> GetById(int id) => Task.FromResult<AbstractReadModel>(null!);
}

public sealed class NonReadModelClass
{
    public static Task<NonReadModelClass> GetById(int id) => Task.FromResult(new NonReadModelClass());
}
