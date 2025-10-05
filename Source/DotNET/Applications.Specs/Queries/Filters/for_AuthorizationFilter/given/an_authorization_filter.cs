// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Cratis.Execution;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Queries.Filters.for_AuthorizationFilter.given;

public class an_authorization_filter : Specification
{
    protected IHttpContextAccessor _httpContextAccessor;
    protected IQueryPerformerProviders _queryPerformerProviders;
    protected AuthorizationFilter _filter;
    protected QueryContext _context;
    protected CorrelationId _correlationId;
    protected HttpContext _httpContext;
    protected ClaimsPrincipal _user;
    protected IQueryPerformer _queryPerformer;

    void Establish()
    {
        _correlationId = CorrelationId.New();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _queryPerformerProviders = Substitute.For<IQueryPerformerProviders>();
        _queryPerformer = Substitute.For<IQueryPerformer>();
        _filter = new AuthorizationFilter(_httpContextAccessor, _queryPerformerProviders);

        _httpContext = Substitute.For<HttpContext>();
        _user = Substitute.For<ClaimsPrincipal>();

        _httpContextAccessor.HttpContext.Returns(_httpContext);
        _httpContext.User.Returns(_user);
    }

    protected void SetupQueryWithoutAuthorization<T>()
        where T : new()
    {
        var queryName = new FullyQualifiedQueryName("TestQuery");
        _queryPerformer.Type.Returns(typeof(T));
        _queryPerformerProviders.TryGetPerformersFor(queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = _queryPerformer;
            return true;
        });

        _context = new QueryContext(queryName, _correlationId, Paging.NotPaged, Sorting.None, null, []);
    }

    protected void SetupQueryWithType(Type queryType)
    {
        var queryName = new FullyQualifiedQueryName("TestQuery");
        _queryPerformer.Type.Returns(queryType);
        _queryPerformerProviders.TryGetPerformersFor(queryName, out var _).Returns(callInfo =>
        {
            callInfo[1] = _queryPerformer;
            return true;
        });

        _context = new QueryContext(queryName, _correlationId, Paging.NotPaged, Sorting.None, null, []);
    }

    protected void SetupQueryWithoutPerformer()
    {
        var queryName = new FullyQualifiedQueryName("TestQuery");
        _queryPerformerProviders.TryGetPerformersFor(queryName, out var _).Returns(false);

        _context = new QueryContext(queryName, _correlationId, Paging.NotPaged, Sorting.None, null, []);
    }

    protected void SetupAuthenticatedUser(params string[] roles)
    {
        var identity = Substitute.For<ClaimsIdentity>();
        identity.IsAuthenticated.Returns(true);
        _user.Identity.Returns(identity);

        foreach (var role in roles)
        {
            _user.IsInRole(role).Returns(true);
        }
    }

    protected void SetupUnauthenticatedUser()
    {
        var identity = Substitute.For<ClaimsIdentity>();
        identity.IsAuthenticated.Returns(false);
        _user.Identity.Returns(identity);
    }

    protected void SetupNoHttpContext()
    {
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);
    }

    protected void SetupNoUser()
    {
        _httpContext.User.Returns((ClaimsPrincipal?)null);
    }
}