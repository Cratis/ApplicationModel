# Arc

As with the backend, you can mix and match from the features you want to use. But there is a convenience wrapper that will help you configure it all in
the form of a custom component that provides the `ArcContext` and configures other Arc contexts in one go.

However, if you're looking to use some of the microservice capabilities, you will have to use the `ArcContext` to provide the name of the
currently running microservice. Internally, Arc uses this information to add the correct headers / query string parameters to distinguish
one microservice from the other in a composition with a single ingress in front of it.

To add Arc, you simply add the following to your application:

```tsx
export const App = () => {
    return (
        <Arc>
            {/* Your application */}
        </Arc>
    );
};
```

## Configuration Options

The `<Arc />` component provides centralized configuration for all commands and queries in your application. These settings apply cross-cuttingly to all operations, eliminating the need to configure individual commands or queries.

| Option | Type | Purpose |
| ------ | ---- | ------- |
| microservice | String | Name of the microservice, which will add necessary HTTP headers on Commands and Queries |
| development | Boolean | Whether or not we're running in development, defaults to false |
| origin | String | Url for where the APIs are located, defaults to empty string and makes them relative to the documents location |
| basePath | String | Base path for the application |
| apiBasePath | String | Base path prepended to all Command and Query requests |
| httpHeadersCallback | Function | Optional callback function that returns additional HTTP headers to include with all commands, queries, and identity requests (e.g., for including cookies or authentication tokens) |

Example:

```tsx
export const App = () => {
    return (
        <Arc apiBasePath="/some/location">
            {/* Your application */}
        </Arc>
    );
};
```

## Microservice Support

In microservice architectures, multiple services often share a single ingress point (e.g., an API gateway or reverse proxy). The `microservice` property enables the Arc to route requests to the correct backend service automatically.

### How It Works

When you specify a microservice name, the Arc adds this information to all HTTP requests (via headers or query parameters), allowing the ingress to route requests appropriately:

```tsx
export const App = () => {
    return (
        <Arc microservice="user-service">
            {/* Your application */}
        </Arc>
    );
};
```

All commands and queries within this application will automatically include the microservice identifier, ensuring they reach the correct backend service.

### Development Configuration

For local development, you can use environment variables to dynamically configure the microservice and API paths:

```tsx
export const App = () => {
    const microserviceName = import.meta.env.VITE_MICROSERVICE_NAME || 'user-service';
    const apiBasePath = import.meta.env.VITE_API_BASE_PATH || '/api';
    const isDevelopment = import.meta.env.DEV;

    return (
        <Arc 
            microservice={microserviceName}
            apiBasePath={apiBasePath}
            development={isDevelopment}>
            {/* Your application */}
        </Arc>
    );
};
```

**.env.development example:**

```env
VITE_MICROSERVICE_NAME=user-service
VITE_API_BASE_PATH=/api
```

**.env.production example:**

```env
VITE_MICROSERVICE_NAME=user-service
VITE_API_BASE_PATH=/api/v1
```

This approach allows you to:

- Run different microservices locally with different configurations
- Switch between local development and remote APIs
- Configure different API paths for different environments
- Test microservice routing without modifying code

### Multiple Microservices in One Frontend

If your frontend application needs to communicate with multiple microservices, you can override the microservice configuration for specific sections of your application using nested `<Arc />` components:

```tsx
export const App = () => {
    return (
        <Arc microservice="main-service" apiBasePath="/api">
            <MainContent />
            
            {/* This section communicates with a different microservice */}
            <Arc microservice="reporting-service">
                <ReportingDashboard />
            </Arc>
        </Arc>
    );
};
```

## HTTP Headers Callback

The `httpHeadersCallback` property allows you to provide additional HTTP headers that will be automatically included with all HTTP requests made by commands, queries, and identity operations. This is particularly useful for including authentication cookies, authorization tokens, or other dynamic headers.

```tsx
export const App = () => {
    const getHeaders = () => {
        return {
            'X-Custom-Header': 'custom-value',
            'Authorization': `Bearer ${getAuthToken()}`,
            // Include cookies or other dynamic headers
        };
    };

    return (
        <Arc 
            apiBasePath="/api" 
            httpHeadersCallback={getHeaders}>
            {/* Your application */}
        </Arc>
    );
};
```

The callback function should return a `HeadersInit` object (compatible with the Fetch API headers) that contains the additional headers to include with each request.
