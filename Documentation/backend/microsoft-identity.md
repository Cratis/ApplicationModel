# Microsoft Identity

Cratis' Application Model provides a way to easily work with providing an object that represents properties the application finds important for describing
the logged in user. The purpose of this is to provide details about the logged in user on the ingress level of an application and letting it
provide the details on the request going in. Having it on the ingress level lets you expose the details to all microservices behind the ingress.

The values provided by the provider are values that are typically application specific and goes beyond what is already found in the token representing the user.
This is optimized for working with Microsoft Azure well known HTTP headers passed on by the different app services, such as Azure ContainerApps or WebApps.
Internally, it is based on the following HTTP headers to be present.

| Header | Description |
| ------ | ----------- |
| x-ms-client-principal | The token holding all the details, base64 encoded [Microsoft Client Principal Data definition](https://learn.microsoft.com/en-us/azure/static-web-apps/user-information?tabs=csharp#client-principal-data) |
| x-ms-client-principal-id | The unique identifier from the identity provider for the identity |
| x-ms-client-principal-name | The name of the identity, typically resolved from claims within the token |

> Important note: Since local development is not configured with the identity provider, but you still need a way to test that both the backend and the frontend
> deals with the identity in the correct way. This can be achieved by creating the correct token and injecting it as request headers using
> a browser extension. Read more [here](../general/generating-principal.md).

The token in the `x-ms-client-principal` should be a base64 encoded [Microsoft Client Principal Data definition](https://learn.microsoft.com/en-us/azure/static-web-apps/user-information?tabs=csharp#client-principal-data).

## Authentication / Authorization

To get the Microsoft Client Principal supported in your backend, the Application Model offers an `AuthenticationHandler` that supports the HTTP headers and
does the right thing to put ASP.NET Core and every `HttpContext` in the right state.

You can add this by calling the `AddMicrosoftIdentityPlatformIdentityAuthentication()` method on your services.

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMicrosoftIdentityPlatformIdentityAuthentication();
```

The above code will then also call the `.AddAuthentication()` with the default scheme name (**MicrosoftIdentityPlatform**) and register
the appropriate `AuthenticationHandler` for that scheme.

You can override the scheme name on the extension method by passing your own string as an argument.

For it to be appropriately setup, you'll need to enable the default authentication and authorization on your app, like below:

```csharp
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
```

## Identity Details

As part of your ingress flow, you can provide additional details for logged in users. On the tokens coming from your identity provider you only have
limited amounts of information and sometimes you want to have more domain specific information.

There are also cases were you need to ask the application if the user is at all authorized to enter the application.

Typically, you would like your ingress to do the composition of this information and create the HTTP headers and cookies needed for this in a single
request without having the frontend do a second request to the server to get more details. And also for the authorization part, you'd like that to happen
before you enter your application and return not authorized if your application is not allowing entry.

If the user is authorized, the ApplicationModel Identity Provider endpoint will put the result as a base64 encoded JSON string on a cookie called `.cratis-identity`. This cookie is then
automatically picked up by the frontend, read more [here](../frontend/react/identity.md). The frontend will then use the cookie if present.

To leverage this mechanism, simply map the endpoint to your application:

```csharp
app.MapIdentityProvider();
```

Once this is done you would simply add code to one of your microservices in your application that provides the additional identity details. You simply
implement the `IProvideIdentityDetails` interface found in the `Cratis.ApplicationModel.Identity` namespace. It will automatically be discovered and
called when needed.

This is unwrapped by the application model and encapsulates it into what is called a `IdentityProviderContext` for you as a developer to consume in a type-safe
manner.

> Note: If your application has just one microservice, you let it implement the `IProvideIdentityDetails` interface.
> For multiple microservices you might want to consider letting your ingress / reverse proxy call all your microservices and merge the results together 
> in one single JSON structure.

Below is an example of an implementation:

```csharp
public class IdentityDetailsProvider : IProvideIdentityDetails
{
    public Task<IdentityDetails> Provide(IdentityProviderContext context)
    {
        var result = new IdentityDetails(true, new { Hello = "World" });
        return Task.FromResult(result);
    }
}
```

The `IdentityProviderContext` holds the following properties:

| Property | Description |
| -------- | ----------- |
| Id | The identity identifier specific from from the identity provider |
| Name | The name of the identity |
| Token | Parsed principal data definition represented as a `JsonObject`|
| Claims | Collection of `KeyValuePair<string, string>` of the claims found in the token |

The code then returns `IdentityDetails` which holds the following properties:

| Property | Description |
| -------- | ----------- |
| IsUserAuthorized | Whether or not the user is authorized into your application or not |
| Details | The actual details in the form of an object, letting you create your own structure |

If the `IsUserAuthorized` property is set to false the return from this will be an HTTP 403. While if it is authorized, a regular HTTP 200.

> Note: Dependency inversion works for this, so your provider can take any dependencies it wants on its constructor.

Your provider will be exposed on a well known route: `/.cratis/me`.
