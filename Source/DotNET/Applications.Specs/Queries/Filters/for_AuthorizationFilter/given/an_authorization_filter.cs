// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Authorization;
using Cratis.Execution;

namespace Cratis.Applications.Queries.Filters.for_AuthorizationFilter.given;

public class an_authorization_filter : Specification
{
    protected IAuthorizationHelper _authorizationHelper;
    protected IQueryPerformerProviders _queryPerformerProviders;
    protected AuthorizationFilter _filter;
    protected QueryContext _context;
    protected CorrelationId _correlationId;
    protected IQueryPerformer _queryPerformer;

    void Establish()
    {
        _correlationId = CorrelationId.New();
        _authorizationHelper = Substitute.For<IAuthorizationHelper>();
        _queryPerformerProviders = Substitute.For<IQueryPerformerProviders>();
        _queryPerformer = Substitute.For<IQueryPerformer>();
        _filter = new AuthorizationFilter(_authorizationHelper, _queryPerformerProviders);
    }
}