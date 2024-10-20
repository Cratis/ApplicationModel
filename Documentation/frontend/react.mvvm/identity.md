# Identity

The MVVM implementation of identity is built on top of what you find the [core](../core/identity.md).
To access identity in an MVVM solution with a view model, the `IdentityProvider` is hooked up to the [container](./tsyringe.md)
through its interface `IIdentityProvider`.

> Note: This depends on the [MVVM Context](./mvvm-context.md) being used.

```typescript
import { injectable } from 'tsyringe';
import { IIdentityProvider } from '@cratis/applications/identity';

type IdentityDetails = {
    department: string,
    age: number
};

@injectable()
export class MyViewModel {
    constructor(private readonly _identityProvider: IIdentityProvider) {
    }

    async sayHello() {
        const identity = await this._identityProvider.getCurrent<IdentityDetails>();
        console.log(`Hello '${identity.name}' from ´${identity.details.department}`);
    }
}
```

The code takes a dependency to the abstract class called `IIdentityProvider` representing the interface for the identity provider.
With this the code simply calls the `getCurrent()` method to get the identity.
