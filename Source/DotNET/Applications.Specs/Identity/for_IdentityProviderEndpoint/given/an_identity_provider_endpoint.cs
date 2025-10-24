// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.given;

public class an_identity_provider_endpoint : Specification
{
    protected JsonSerializerOptions _serializerOptions;
    protected IProvideIdentityDetails _identityProvider;
    protected IdentityProviderEndpoint _endpoint;
    protected HttpRequest _request;
    protected HttpResponse _response;
    protected HttpContext _httpContext;

    void Establish()
    {
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();

        _serializerOptions = new JsonSerializerOptions();
        _identityProvider = Substitute.For<IProvideIdentityDetails>();
        _endpoint = new(_serializerOptions, _identityProvider);

        _request = _httpContext.Request;
        _response = _httpContext.Response;
    }
}
