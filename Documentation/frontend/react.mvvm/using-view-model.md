# Using a View Model

Every React functional component can have a view model. This is accomplished using the `withViewModel()` method.

```tsx
export const Counter = withViewModel(CounterViewModel, ({viewModel}) => {
    return (
        <>
            Counter is : {viewModel.counter}
            <button onClick={() => viewModel.increaseCounter()}>Increase counter</button>
        </>
    )

});
```

The component uses the `withViewModel` and passes the type `CounterViewModel` to be created for the view.
As arguments you can then have the `viewModel`, this instance survives re-renders and can be stateful.

```ts
export class CounterViewModel {
    counter: number;

    increaseCounter() {
        this.counter++;
    }
}
```

The `viewModel` is automatically observable, which means that all properties on it will notify the view
if there are any changes to them. This means that the `increaseCounter()` method can just go ahead and
increase the counter and the view will automatically re-render.

## Params

Params defined as part of routes using `react-router` can easily be accessed by a view model in a couple of ways.

### Injecting Params as dependency

If you're component only gets loaded once and the parameters can't change without the component being unloaded
and then loaded again, you can simply inject it as part of the constructor:

```ts
import { WellKnownBindings } from '@cratis/applications.react.mvvm';
import { inject, injectable } from 'tsyringe';

@injectable()
export class MyViewModel {
    constructor(@inject(WellKnownBindings.params) params: Params) {
    }
}
```

The downside of this approach is that if a param changes and the component is not unloaded, you won't get the
change.

### Handling Params

For the scenario were the params are changing while the component is not unloaded, implementing the `IHandleParams<>`
interface is a better option. It is a generic interface, but the generic argument is optional and is defaulted to
`object` if not specified.

```ts
import { IHandleParams } from '@cratis/applications.react.mvvm';

export class MyViewModel implements IHandleParams<Params>  {
    handleParams(params: Params): void {
        // Do things based on params
    }
}
```

> Note: The `handleParams` method will be called both on initial load and for any subsequent changes.

## QueryParams

QueryParams defined as part of routes using `react-router` can easily be accessed by a view model in a couple of ways.

### Injecting Query Params as dependency

If you're component only gets loaded once and the query parameters can't change without the component being unloaded
and then loaded again, you can simply inject it as part of the constructor:

```ts
import { WellKnownBindings } from '@cratis/applications.react.mvvm';
import { inject, injectable } from 'tsyringe';

@injectable()
export class MyViewModel {
    constructor(@inject(WellKnownBindings.queryParams) queryParams: Params) {
    }
}
```

The downside of this approach is that if a query param changes and the component is not unloaded, you won't get the
change.

### Handling Query Params

For the scenario were the query params are changing while the component is not unloaded, implementing the `IHandleParams<>`
interface is a better option. It is a generic interface, but the generic argument is optional and is defaulted to
`object` if not specified.

```ts
import { IHandleQueryParams } from '@cratis/applications.react.mvvm';

export class MyViewModel implements IHandleQueryParams<Params>  {
    handleQueryParams(queryParams: QueryParams): void {
        // Do things based on query params
    }
}
```

> Note: The `handleQueryParams` method will be called both on initial load and for any subsequent changes.

## View Model lifecycle

### Detaches

You can get notified when a view model is detached from its view, typically as a consequence of the view being removed from the DOM.
This is achieved by implementing the `IViewModelDetached` interface on your view model:

```ts
import { IViewModelDetached } from '@cratis/applications.react.mvvm';

export class MyViewModel implements IViewModelDetached {
    detached() {
        // Clean up...
    }
}
```
