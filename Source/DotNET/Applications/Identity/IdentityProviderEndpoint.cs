// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Identity;

/// <summary>
/// Represents the actual endpoint called for identity details (/.cratis/me).
/// </summary>
public class IdentityProviderEndpoint
{
    readonly JsonSerializerOptions _serializerOptions;
    readonly IProvideIdentityDetails _identityProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityProviderEndpoint"/> class.
    /// </summary>
    /// <param name="serializerOptions"><see cref="JsonSerializerOptions"/> to use for serialization.</param>
    /// <param name="identityProvider"><see cref="IProvideIdentityDetails"/> for providing the identity.</param>
    public IdentityProviderEndpoint(JsonSerializerOptions serializerOptions, IProvideIdentityDetails identityProvider)
    {
        _serializerOptions = new JsonSerializerOptions(serializerOptions)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        _identityProvider = identityProvider;
    }

    /// <summary>
    /// Handle the identity request.
    /// </summary>
    /// <param name="request"><see cref="HttpRequest"/> that holds all the request information.</param>
    /// <param name="response"><see cref="HttpResponse"/> that will be written to.</param>
    /// <returns>Awaitable task.</returns>
    public async Task Handler(HttpRequest request, HttpResponse response)
    {
        if (request.IsValidIdentityRequest())
        {
            IdentityId identityId = request.Headers[MicrosoftIdentityPlatformHeaders.IdentityIdHeader].ToString();
            IdentityName identityName = request.Headers[MicrosoftIdentityPlatformHeaders.IdentityNameHeader].ToString();

            var clientPrincipal = request.GetClientPrincipal();
            if (clientPrincipal is not null)
            {
                var token = Convert.FromBase64String(request.Headers[MicrosoftIdentityPlatformHeaders.PrincipalHeader].ToString());
                var tokenAsJson = JsonNode.Parse(token) as JsonObject;
                var claims = request.GetClaims().Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value));
                var context = new IdentityProviderContext(identityId, identityName, tokenAsJson!, claims);
                var result = await _identityProvider.Provide(context);
                IdentityProviderResult identityResult;

                if (result.IsUserAuthorized)
                {
                    response.StatusCode = 200;
                    identityResult = new IdentityProviderResult(context.Id, context.Name, context.Claims, result.Details);
                }
                else
                {
                    response.StatusCode = 403;
                    identityResult = new IdentityProviderResult(string.Empty, string.Empty, [], new { });
                }

                response.ContentType = "application/json; charset=utf-8";
                await response.WriteAsJsonAsync(identityResult, _serializerOptions);
            }
        }
    }
}
