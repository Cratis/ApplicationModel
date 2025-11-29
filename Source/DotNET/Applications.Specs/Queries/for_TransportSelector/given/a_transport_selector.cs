// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Queries.for_TransportSelector.given;

public class a_transport_selector : Specification
{
    protected TransportSelector _selector;
    protected ObservableQueryTransportOptions _options;
    protected DefaultHttpContext _httpContext;
    protected IHttpWebSocketFeature _webSocketFeature;

    void Establish()
    {
        _options = new ObservableQueryTransportOptions();
        _selector = new TransportSelector(Options.Create(_options));
        _httpContext = new DefaultHttpContext();
        _webSocketFeature = Substitute.For<IHttpWebSocketFeature>();
        _httpContext.Features.Set(_webSocketFeature);
    }
}
