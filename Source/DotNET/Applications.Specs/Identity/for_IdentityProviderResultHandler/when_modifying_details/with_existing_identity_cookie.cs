// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Identity.for_IdentityProviderResultHandler.when_modifying_details;

public class with_existing_identity_cookie : given.an_identity_provider_result_handler
{
    TestDetails _originalDetails;
    TestDetails _modifiedDetails;
    IdentityProviderResult<TestDetails> _originalResult;
    string _base64Json;
    string _responseContent;
    IdentityProviderResult _responseResult;
    Microsoft.Net.Http.Headers.SetCookieHeaderValue _identityCookie;
    IdentityProviderResult _cookieResult;

    void Establish()
    {
        _originalDetails = new TestDetails { Department = "Engineering", Role = "Developer" };
        _modifiedDetails = new TestDetails { Department = "Marketing", Role = "Manager" };

        _originalResult = new IdentityProviderResult<TestDetails>(
            new IdentityId("user123"),
            new IdentityName("Test User"),
            true,
            true,
            _originalDetails);

        var json = JsonSerializer.Serialize(_originalResult, _serializerOptions);
        _base64Json = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        _httpContext.Request.Cookies = new TestRequestCookieCollection();
        ((TestRequestCookieCollection)_httpContext.Request.Cookies).Add(IdentityProviderResultHandler.IdentityCookieName, _base64Json);

        _httpContext.Request.Scheme = "https";
    }

    async Task Because()
    {
        await _handler.ModifyDetails<TestDetails>(details => _modifiedDetails);

        _httpContext.Response.Body.Position = 0;
        var reader = new StreamReader(_httpContext.Response.Body);
        _responseContent = await reader.ReadToEndAsync();
        _responseResult = JsonSerializer.Deserialize<IdentityProviderResult>(_responseContent, _serializerOptions)!;

        var cookies = _httpContext.Response.GetTypedHeaders().SetCookie;
        _identityCookie = cookies.FirstOrDefault(c => c.Name == IdentityProviderResultHandler.IdentityCookieName)!;

        var cookieValue = _identityCookie.Value.ToString();
        var urlDecodedValue = Uri.UnescapeDataString(cookieValue);
        var decodedJson = Encoding.UTF8.GetString(Convert.FromBase64String(urlDecodedValue));
        _cookieResult = JsonSerializer.Deserialize<IdentityProviderResult>(decodedJson, _serializerOptions)!;
    }

    [Fact] void should_write_modified_result_to_response()
    {
        var detailsJson = JsonSerializer.Serialize(_responseResult.Details, _serializerOptions);
        var expectedDetailsJson = JsonSerializer.Serialize(_modifiedDetails, _serializerOptions);
        detailsJson.ShouldEqual(expectedDetailsJson);
    }

    [Fact] void should_set_new_identity_cookie_with_modified_details()
    {
        _identityCookie.ShouldNotBeNull();
        var detailsJson = JsonSerializer.Serialize(_cookieResult.Details, _serializerOptions);
        var expectedDetailsJson = JsonSerializer.Serialize(_modifiedDetails, _serializerOptions);
        detailsJson.ShouldEqual(expectedDetailsJson);
    }

    public class TestDetails
    {
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}