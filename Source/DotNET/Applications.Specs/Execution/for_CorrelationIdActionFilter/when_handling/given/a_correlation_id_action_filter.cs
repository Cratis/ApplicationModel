// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Execution.for_CorrelationIdActionFilter.when_handling.given;

public class a_correlation_id_action_filter : Specification
{
    protected CorrelationIdActionFilter _correlationIdActionFilter;
    protected ActionExecutingContext _actionExecutingContext;
    protected ActionExecutionDelegate _next;
    protected IOptions<ApplicationModelOptions> _options;
    protected ICorrelationIdAccessor _correlationIdAccessor;
    protected ICorrelationIdModifier _correlationIdModifier;
    protected HttpContext _httpContext;
    protected HttpRequest _httpRequest;
    protected IHeaderDictionary _headers;
    protected CorrelationIdOptions _correlationIdOptions;
    protected CorrelationId _currentCorrelationId;

    void Establish()
    {
        _correlationIdOptions = new CorrelationIdOptions
        {
            HttpHeader = Constants.DefaultCorrelationIdHeader
        };
        _options = Options.Create(new ApplicationModelOptions
        {
            CorrelationId = _correlationIdOptions
        });
        _correlationIdAccessor = Substitute.For<ICorrelationIdAccessor, ICorrelationIdModifier>();
        _correlationIdModifier = _correlationIdAccessor as ICorrelationIdModifier;
        _currentCorrelationId = CorrelationId.NotSet;
        _correlationIdAccessor.Current.Returns(_ => _currentCorrelationId);
        _correlationIdModifier
            .When(c => c.Modify(Arg.Any<CorrelationId>()))
            .Do(c => _currentCorrelationId = c.Arg<CorrelationId>());

        _httpContext = Substitute.For<HttpContext>();
        _httpRequest = Substitute.For<HttpRequest>();
        _httpContext.Request.Returns(_httpRequest);
        _headers = Substitute.For<IHeaderDictionary>();
        _httpRequest.Headers.Returns(_headers);
        var actionContext = new ActionContext(_httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ControllerActionDescriptor());
        _actionExecutingContext = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), null!);
        _correlationIdActionFilter = new CorrelationIdActionFilter(_options, _correlationIdAccessor);
        _next = Substitute.For<ActionExecutionDelegate>();
    }
}
