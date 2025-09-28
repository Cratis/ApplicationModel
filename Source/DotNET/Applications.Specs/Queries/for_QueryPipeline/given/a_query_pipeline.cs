// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Queries.for_QueryPipeline.given;

public class a_query_pipeline : Specification
{
    protected QueryPipeline pipeline;
    protected ICorrelationIdAccessor correlation_id_accessor;
    protected IQueryContextManager query_context_manager;
    protected IQueryFilters query_filters;
    protected IQueryPerformerProviders query_performer_providers;
    protected IQueryRenderers query_renderers;
    protected IServiceProvider service_provider;
    protected IQueryPerformer query_performer;
    protected CorrelationId correlation_id;

    void Establish()
    {
        correlation_id = CorrelationId.New();

        correlation_id_accessor = Substitute.For<ICorrelationIdAccessor>();
        correlation_id_accessor.Current.Returns(correlation_id);

        query_context_manager = Substitute.For<IQueryContextManager>();
        query_filters = Substitute.For<IQueryFilters>();
        query_performer_providers = Substitute.For<IQueryPerformerProviders>();
        query_renderers = Substitute.For<IQueryRenderers>();
        service_provider = Substitute.For<IServiceProvider>();
        query_performer = Substitute.For<IQueryPerformer>();

        pipeline = new QueryPipeline(
            correlation_id_accessor,
            query_context_manager,
            query_filters,
            query_performer_providers,
            query_renderers,
            service_provider);
    }
}