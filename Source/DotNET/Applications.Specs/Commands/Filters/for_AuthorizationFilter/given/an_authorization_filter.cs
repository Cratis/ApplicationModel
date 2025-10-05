// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Cratis.Execution;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Commands.Filters.for_AuthorizationFilter.given;

public class an_authorization_filter : Specification
{
    protected IHttpContextAccessor _httpContextAccessor;
    protected AuthorizationFilter _filter;
    protected CommandContext _context;
    protected CorrelationId _correlationId;
    protected HttpContext _httpContext;
    protected ClaimsPrincipal _user;

    void Establish()
    {
        _correlationId = CorrelationId.New();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _filter = new AuthorizationFilter(_httpContextAccessor);

        _httpContext = Substitute.For<HttpContext>();
        _user = Substitute.For<ClaimsPrincipal>();

        _httpContextAccessor.HttpContext.Returns(_httpContext);
        _httpContext.User.Returns(_user);
    }

    protected void SetupCommandWithoutAuthorization<T>()
        where T : new()
    {
        var command = new T();
        _context = new CommandContext(_correlationId, typeof(T), command, [], new());
    }

    protected void SetupCommandWithType(Type commandType, object command)
    {
        _context = new CommandContext(_correlationId, commandType, command, [], new());
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