# Dialogs

Working with dialogs is different when decoupling your code as you do with the MVVM paradigm.
Even though the view models are not responsible for rendering and should be blissfully unaware of how things gets rendered,
you do need at times to interact with the user.

Cratis Application Model supports an approach to working with dialogs and still maintain the clear separation of concerns.
It promotes the idea of letting the view and React as a rendering library do just that and then bridges everything through a
service called `IDialogs` and the use of specific hooks to glue it together, making it feel natural for you as a React developer
whilst having a clear separation and making your view model logic clear and concise.

The beauty of this is that you can quite easily also write automated unit tests that test for the scenarios, involving dialogs.

## Confirmation Dialogs

The most common use of modal dialogs are the standard confirmation dialogs. These are dialogs where you ask the user to confirm
a specific action. The Application Model supports these out of the box and you have options for what type of confirmation you're
looking for in the form of passing it which buttons to show.

There is an enum called `DialogButtons` that has the following options:

| Value | Description |
| ----- | ----------- |
| Ok    | Only show a single Ok button, typically used to inform the user and the user to acknowledge |
| OkCancel | Show both an Ok and a Cancel button |
| YesNo | Show a Yes and No button |
| YesNoCancel | Show Yes, No and a Cancel button |

For standard confirmation dialogs, there is a specific expected result called `DialogResult` that the dialog needs to communicate back.
The values are:

* Yes
* No
* Ok
* Cancel

To use a confirmation dialog from a ViewModel, you need to take a dependency to the `IDialogs`, assuming you have hooked up [TSyringe and bindings](./tsyringe.md).
Then in a method, you can call the `showConfirmation()` on the `IDialogs` to show the confirmation.

Below is a full sample of how this works.

```ts
import { injectable } from 'tsyringe';
import { DialogResult } from '@cratis/applications.react/dialogs';
import { DialogButtons, IDialogs } from '@cratis/applications.react.mvvm/dialogs';

@injectable()
export class YourViewModel {
    constructor(
        private readonly _dialogs: IDialogs) {
    }

    // Method called from typically your view
    async deleteTheThing() {
        const result = await this._dialogs.showConfirmation('Delete?', 'Are you sure you want to delete?', DialogButtons.YesNo);
        if( result == DialogResult.Yes ) {
            // Do something - typically call your server
        }
    }
}
```

Since you haven't defined how a confirmation dialog looks like, you will not see anything, nor will the `showConfirmation()` ever return.

### Defining the Confirmation Dialog

You define a confirmation dialog on the application level. It is then the dialog that will be used across your entire application.

The anatomy of any dialog is that it uses the `useDialogContext()` in the dialog itself to get what context it is in.
With the context you get access to the actual request payload and the method to call when the dialog should close, or the dialog resolver
as it is called.

Below is an example using [Prime React](http://primereact.org) to create a confirmation dialog supporting the different button types.

```tsx
import { Dialog } from 'primereact/dialog';
import { DialogButtons, ConfirmationDialogRequest, useDialogContext } from '@cratis/applications.react.mvvm/dialogs';
import { DialogResult } from '@cratis/applications.react/dialogs';
import { Button } from 'primereact/button';

export const ConfirmationDialog = () => {
    const { request, resolver } = useDialogContext<ConfirmationDialogRequest, DialogResult>();

    const headerElement = (
        <div className="inline-flex align-items-center justify-content-center gap-2">
            <span className="font-bold white-space-nowrap">{request.title}</span>
        </div>
    );

    const okFooter = (
        <>
            {/* Hook up buttons with resolvers resolving to expected DialogResult */}
            <Button label="Ok" icon="pi pi-check" onClick={() => resolver(DialogResult.Ok)} autoFocus />
        </>
    );

    const okCancelFooter = (
        <>
            {/* Hook up buttons with resolvers resolving to expected DialogResult */}
            <Button label="Ok" icon="pi pi-check" onClick={() => resolver(DialogResult.Ok)} autoFocus />
            <Button label="Cancel" icon="pi pi-times" severity='secondary' onClick={() => resolver(DialogResult.Cancelled)} />
        </>
    );

    const yesNoFooter = (
        <>
            {/* Hook up buttons with resolvers resolving to expected DialogResult */}
            <Button label="Yes" icon="pi pi-check" onClick={() => resolver(DialogResult.Yes)} autoFocus />
            <Button label="No" icon="pi pi-times" severity='secondary' onClick={() => resolver(DialogResult.No)} />
        </>
    );

    const yesNoCancelFooter = (
        <>
            {/* Hook up buttons with resolvers resolving to expected DialogResult */}
            <Button label="Yes" icon="pi pi-check" onClick={() => resolver(DialogResult.Yes)} autoFocus />
            <Button label="No" icon="pi pi-times" severity='secondary' onClick={() => resolver(DialogResult.No)} />
        </>
    );

    const getFooterInterior = () => {
        switch (request.buttons) {
            case DialogButtons.Ok:
                return okFooter;
            case DialogButtons.OkCancel:
                return okCancelFooter;
            case DialogButtons.YesNo:
                return yesNoFooter;
            case DialogButtons.YesNoCancel:
                return yesNoCancelFooter;
        }

        return (<></>)
    }

    const footer = (
        <div className="card flex flex-wrap justify-content-center gap-3">
            {getFooterInterior()}
        </div>
    );

    return (
        <>
            {/* On hide we call the resolver with cancelled */}
            <Dialog header={headerElement} modal footer={footer} onHide={() => resolver(DialogResult.Cancelled)} visible={true}>
                <p className="m-0">
                    {request.message}
                </p>
            </Dialog>
        </>
    );
};
```

The code above uses the `useDialogContext()` with the `ConfirmationDialogRequest` and `DialogResult` as the types expected from
the request and resolver type. Within the rendering of the component you'll notice that buttons are hooked up to resolve the
dialog with the expected `DialogResult`. Once a button is called, it resolves the request and the information will be passed
onto the `Promise` created within the `IDialogs` service.

To enable the new `ConfirmationDialog` all you need to do is hook it up in your application like below.

```tsx
export const App = () => {
    return (
        <DialogComponents confirmation={ConfirmationDialog}>
            {/* Your application */}
        </DialogComponents>
    );
};
```

## Busy indicator dialogs

Another common type of modal dialog is the indeterminate busy indicator dialog. You typically use these dialogs for giving
a visual clue to the user that the system is working. These type of dialogs are not meant to be something the user can
close, but rather something the system closes when it is ready with the work the system is doing.

To use a busy indicator dialog from a ViewModel, you need to take a dependency to the `IDialogs`, assuming you have hooked up [TSyringe and bindings](./tsyringe.md).
Then in a method, you can call the `showBusyIndicator()` on the `IDialogs` to show the confirmation.

Below is a full sample of how this works.

```ts
import { injectable } from 'tsyringe';
import { DialogResult } from '@cratis/applications.react/dialogs';
import { DialogButtons, IDialogs } from '@cratis/applications.react.mvvm/dialogs';

@injectable()
export class YourViewModel {
    constructor(
        private readonly _dialogs: IDialogs) {
    }

    // Method called from typically your view
    async performLongRunningOperation() {
        const busyIndicator = this._dialogs.showBusyIndicator('Performing something that will take a while', 'Please wait');
        setTimeout(() => {
            busyIndicator.close();
        }, 1000);
    }
}
```

The `showBusyIndicator()` returns an object that has a method called `close()`. This method is then something you use to close the dialog.

Since you haven't defined how a busy indicator dialog looks like, you will not see anything.

### Defining the Busy Indicator Dialog

As with the confirmation dialog, you define a busy indicator dialog on the application level. It is then the dialog that will be used across your entire application.

The anatomy of any dialog is that it uses the `useDialogContext()` in the dialog itself to get what context it is in.
With the context you get access to the actual request payload and the method to call when the dialog should close, or the dialog resolver
as it is called.

Below is an example using [Prime React](http://primereact.org) to create a confirmation dialog supporting the different button types.


```tsx
import { Dialog } from 'primereact/dialog';
import { useDialogContext, BusyIndicatorDialogRequest } from '@cratis/applications.react.mvvm/dialogs';
import { DialogResult } from '@cratis/applications.react/dialogs';
import { ProgressSpinner } from 'primereact/progressspinner';

export const BusyIndicatorDialog = () => {
    const { request } = useDialogContext<BusyIndicatorDialogRequest, DialogResult>();

    const headerElement = (
        <div className="inline-flex align-items-center justify-content-center gap-2">
            <span className="font-bold white-space-nowrap">{request.title}</span>
        </div>
    );

    return (
        <>
            <Dialog header={headerElement} modal visible={true} onHide={() => { }}>
                <ProgressSpinner />
                <p className="m-0">
                    {request.message}
                </p>
            </Dialog>
        </>
    );
};
```

The code above uses the `useDialogContext()` with the `BusyIndicatorDialogRequest` and `DialogResult` as the types expected from
the request and resolver type. For this implementation, it shows a spinner.

To enable the new `BusyIndicatorDialog` all you need to do is hook it up in your application like below.

```tsx
export const App = () => {
    return (
        <DialogComponents busyIndicator={BusyIndicatorDialog}>
            {/* Your application */}
        </DialogComponents>
    );
};
```

## Custom dialogs

The anatomy of dialogs in general is based on a **request** and **response** pattern.
You request a dialog through the `IDialogs` service by giving it an instance of a type of a message that the view knows
how to resolve into a dialog. This mechanism is in use on the confirmation dialogs and is the same for a custom dialog.

For the dialog to know the context in which it is rendering, there is a hook called `useDialogContext()`.
In the view where the dialog is used, you define the context implicitly by using the `useDialogRequest()`.
This establishes the **subscriber** that responds to a request from your view model of showing a dialog.

Subscriptions are based on type and it must be a well known type at runtime, so typically in TypeScript you'd define the
request as a class as `interface` and `type` is optimized away by the TypeScript transpiler and are not present at runtime.

The following code creates a custom dialog component.

```tsx
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { useDialogContext } from '@cratis/applications.react.mvvm/dialogs';

export class CustomDialogRequest { 
    constructor(readonly content: string) {
    }
}

export const CustomDialog = () => {
    const { request, resolver } = useDialogContext<CustomDialogRequest, string>();

    return (
        <Dialog header="My custom dialog" visible={true} onHide={() => resolver('Did not do it..')}>
            <h2>Dialog</h2>
            {request.content}
            <br />
            <Button onClick={() => resolver('Done done done...')}>We're done</Button>
        </Dialog>
    );
};
```

Notice that the code creates a `CustomDialogRequest` class, it is defined as an immutable class with a constructor that
holds the properties as `readonly`. The purpose of the the request object is to provide information that can be passed along
from a view model to the dialog. This could for instance be data is needed to be displayed in the dialog or similar.
You don't need to have any properties on it, the type as a class is however required.

Within the dialog component, you use the `useDialogContext()` and pass it the request type and the expected response type.
The hook returns an object called `IDialogContext`, this holds the request and a delegate type that can be called to
"resolve" the dialog. Both properties are type-safe based on the generic parameters passed to the hook.

With the custom dialog defined, we can start using it.

Below is an example of a view that leverages the dialog and has a view model behind that actually shows it.

```tsx
import { withViewModel } from '@cratis/applications.react.mvvm';
import { FeatureViewModel } from './FeatureViewModel';
import { useDialogRequest } from '@cratis/applications.react.mvvm/dialogs';
import { CustomDialog, CustomDialogRequest } from './CustomDialog';

export const Feature = withViewModel<FeatureViewModel>(FeatureViewModel, ({ viewModel }) => {

    // Use the dialog request to get a wrapper for rendering our dialog
    const [CustomDialogWrapper] = useDialogRequest<CustomDialogRequest, string>(CustomDialogRequest);

    return (
        <div>
            {/* Use the dialog wrapper here. It will automatically show or hide its children - your dialog */}
            <CustomDialogWrapper>
                <CustomDialog />
            </CustomDialogWrapper>
        </div>
    );
});
```

The code leverages the `useDialogRequest()` with the generic parameters corresponding to the request and response types,
as you saw when defining the `CustomDialog` component. It returns a **tuple** that holds a wrapper as a React functional component,
then the context which holds the request when a request is made and then a resolver. This allows for inlining dialogs or passing the information
on to things that needs it. But for this scenario, we don't need them and we therefor only capture the wrapper.

> Note: See the sample later on how to create dialogs with a view model for an example of context and resolver use.

With the wrapper, the code wraps the actual `CustomDialog` component as part of the rendering of the component. This ensures that
is will only be displayed when it is supposed to.

The last piece of the puzzle is now to use it from the view model. Following is a sample that shows the usage.

```ts
import { injectable } from 'tsyringe';
import { DialogButtons, IDialogs } from '@cratis/applications.react.mvvm/dialogs';
import { CustomDialogRequest } from './CustomDialogRequest;

@injectable()
export class FeatureViewModel {
    constructor(
        private readonly _dialogs: IDialogs) {
    }

    async doThings() {
        // Show the custom dialog
        const result = await this._dialogs.show<CustomDialogRequest, string>(new CustomDialogRequest('This is the content to show));
        if( result == 'Done done done...') {
            // Do something
        }
    }
}
```

The view model takes a dependency to `IDialogs` which is resolved by the IoC, assuming you have hooked up [TSyringe and bindings](./tsyringe.md).

In the `doThings()` method we show the dialog by calling `.show()` on the `IDialogs` service, giving it an instance of the
`CustomDialogRequest`. With the generic arguments; `CustomDialogRequest` and `string` we are sure to get type-safety for the response.
If you don't provide any of the generic arguments, the return type will become `unknown`.

The `.show()` method is an async, `Promise` based method that will return when the dialog is resolved.
The return from the `.show()` method will then be the response type, in this case; **a string**.

### Dialog with a view model

You might want to use a view model for the dialog itself. That is fully possible and recommended for scenarios where there will be logic
and / or you want to be able to test the dialog logic code.

Your dialog view would then look like below:

```tsx
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { useDialogContext } from '@cratis/applications.react.mvvm/dialogs';
import { DialogResolver } from '@cratis/applications.react.mvvm/dialogs';
import { withViewModel } from '@cratis/applications.react.mvvm';
import { CustomDialogViewModel } from './CustomDialogViewModel';

// The dialog now needs props with the request and resolver.
export interface CustomDialogProps {
    request: CustomDialogRequest;
    resolver: DialogResolver<string>;
}

export class CustomDialogRequest { 
    constructor(readonly content: string) {
    }
}

// Use the withViewModel() to pull in and specify props
export const CustomDialog = withViewModel<CustomDialogViewModel, CustomDialogProps>({ viewModel, props }) => {
    return (
        <Dialog header="My custom dialog" visible={true} onHide={() => viewModel.cancel() }>
            <h2>Dialog</h2>
            {request.content}
            <br />
            <Button onClick={() => viewModel.done() }>We're done</Button>
        </Dialog>
    );
};
```

The above code introduces a `CustomDialogProps` type that holds the request and resolver. This is now something you need to pass down to
the component, mostly for the view model to be able to get these as that is not available directly to the view model.

Your view model would then need to be something like below:

```ts
import { inject, injectable } from 'tsyringe';
import { type CustomDialogProps } from './CustomDialog';

@injectable()
export class CustomDialogViewModel {

    constructor(@inject('props') private readonly _props: CustomDialogProps) {
    }

    name: string = '';

    done() {
        this._props.resolver('Done done done...');
    }

    cancel() {
        this._props.resolver('Did not do it..');
    }
}
```

The view model now has a *named* dependency called `props`, this is automatically hooked up for the component. It contains
the resolver and our view model can now handle the logic.

Using the dialog is almost exactly the same, but now we need to provide the context and resolver:

```tsx
import { withViewModel } from '@cratis/applications.react.mvvm';
import { FeatureViewModel } from './FeatureViewModel';
import { useDialogRequest } from '@cratis/applications.react.mvvm/dialogs';
import { CustomDialog, CustomDialogRequest } from './CustomDialog';

export const Feature = withViewModel<FeatureViewModel>(FeatureViewModel, ({ viewModel }) => {

    // Use the dialog request to get a wrapper for rendering our dialog and the context and resolver
    const [CustomDialogWrapper, context, resolver] = useDialogRequest<CustomDialogRequest, string>(CustomDialogRequest);

    return (
        <div>
            {/* Use the dialog wrapper here. It will automatically show or hide its children - your dialog */}
            <CustomDialogWrapper>
                <CustomDialog request={context.request} resolver={resolver}/>
            </CustomDialogWrapper>
        </div>
    );
});
```

The code takes all the values of the tuple returned by `useDialogRequest()` and passes the request from the context and resolver
into the `CustomDialog` component, as it will use this in its view model.
