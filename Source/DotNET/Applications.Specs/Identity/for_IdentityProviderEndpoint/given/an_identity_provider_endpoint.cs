// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.given;

public class an_identity_provider_endpoint : Specification
{
    protected JsonSerializerOptions serializer_options;
    protected Mock<IProvideIdentityDetails> identity_provider;
    protected IdentityProviderEndpoint endpoint;
    protected Mock<HttpRequest> request;
    protected Mock<HttpResponse> response;
    protected HeaderDictionary headers;
    protected MemoryStream body_stream;
    protected Mock<HttpContext> http_context;

    void Establish()
    {
        http_context = new();
        serializer_options = new JsonSerializerOptions();
        identity_provider = new();
        endpoint = new(serializer_options, identity_provider.Object);

        request = new();
        request.SetupGet(_ => _.HttpContext).Returns(http_context.Object);
        headers = [];
        request.SetupGet(_ => _.Headers).Returns(headers);

        response = new();
        response.SetupGet(_ => _.HttpContext).Returns(http_context.Object);
        body_stream = new();
        response.SetupGet(_ => _.Body).Returns(body_stream);
    }
}
