# Using a View Model

Every React functional component can have a view model. This is accomplished using the `withViewModel()` method.

## View Model lifecycles

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
