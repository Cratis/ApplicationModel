// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Queries;
using Cratis.Execution;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet;

public class QueryContextBuilder(CorrelationId correlationId)
{
    Paging _paging = Paging.NotPaged;
    Sorting _sorting = Sorting.None;

    public static QueryContextBuilder New() => new(CorrelationId.New());

    public QueryContextBuilder WithPageSize(int pageSize)
    {
        _paging = new(0, pageSize, true);
        return this;
    }

    public QueryContextBuilder WithSorting(Sorting sorting)
    {
        _sorting = sorting;
        return this;
    }

    public QueryContext Build() => new("[Test]", correlationId, _paging, _sorting);
}
