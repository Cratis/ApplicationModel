// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Queries.ModelBound.for_ModelBoundQueryPerformer.given;

public class a_model_bound_query_performer : Specification
{
    protected ModelBoundQueryPerformer _performer;
    protected QueryContext _context;
    protected object? _result;
    protected IServiceProviderIsService _serviceProviderIsService;

    void Establish()
    {
        _serviceProviderIsService = Substitute.For<IServiceProviderIsService>();
    }

    protected void EstablishPerformer<T>(string methodName, object[]? dependencies = null, QueryArguments? parameters = null)
    {
        var method = typeof(T).GetMethod(methodName);
        _performer = new ModelBoundQueryPerformer(typeof(T), method!, _serviceProviderIsService);

        parameters ??= new QueryArguments();
        dependencies ??= [];

        _context = new QueryContext(
            _performer.FullyQualifiedName,
            CorrelationId.New(),
            Paging.NotPaged,
            Sorting.None,
            parameters,
            dependencies);
    }

    protected async Task PerformQuery() => _result = await _performer.Perform(_context);
}