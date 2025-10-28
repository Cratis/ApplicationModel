// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Identity;

/// <summary>
/// Represents the actual endpoint called for identity details (/.cratis/me).
/// </summary>
/// <param name="serializerOptions"><see cref="JsonSerializerOptions"/> to use for serialization.</param>
/// <param name="identityProvider"><see cref="IProvideIdentityDetails"/> for providing the identity.</param>
public class IdentityProviderEndpoint(JsonSerializerOptions serializerOptions, IProvideIdentityDetails identityProvider)
{
    const string IdentityCookieName = ".cratis.identity";

    readonly JsonSerializerOptions _serializerOptions = new(serializerOptions)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Handle the identity request.
    /// </summary>
    /// <param name="request"><see cref="HttpRequest"/> that holds all the request information.</param>
    /// <param name="response"><see cref="HttpResponse"/> that will be written to.</param>
    /// <returns>Awaitable task.</returns>
    public async Task Handler(HttpRequest request, HttpResponse response)
    {
        if (!request.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            response.StatusCode = 401;
            return;
        }

        var claimsPrincipal = request.HttpContext.User;
        if (claimsPrincipal is not null)
        {
            var identityId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "unknown";
            var identityName = claimsPrincipal.Identity?.Name ?? "unknown";
            var claims = claimsPrincipal.Claims.Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value));

            var context = new IdentityProviderContext(identityId, identityName, claims);
            var result = await identityProvider.Provide(context);
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
            var json = JsonSerializer.Serialize(identityResult, _serializerOptions);
            var base64Json = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
            response.Cookies.Append(IdentityCookieName, base64Json, new CookieOptions
            {
                HttpOnly = false,
                Secure = request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            await response.WriteAsync(json);
        }
    }
}
