# Dialogs

Cratis Application Model provides a consistent way of working with dialogs independent of component libraries.
The purpose is to promote a way of working with dialogs as separate components and not something you
do inline within another component.

## Confirmation Dialogs

A common use of modal dialogs are the standard confirmation dialogs. These are dialogs where you ask the user to confirm
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

There is a custom React hook for leveraging this type of dialog.

```tsx
import { Button } from 'primereact/button';
import { DialogButtons, useConfirmationDialog } from '@cratis/applications.react/dialogs';

export const Feature = () => {
    const [showConfirmationDialog] = useConfirmationDialog('The title', 'The message', DialogButtons.YesNo);

    return (
        <div>
            <Button onClick={() => showConfirmationDialog() }/>
        </div>
    )
}
```

The code uses the `useConfirmationDialog` hook that returns a function for showing the dialog.
It passes the definition of the dialog as parameters to the hook, setting the title, the message and what buttons to show.

Alternatively to defining the dialog on the hook level, you can also pass it along on the `showConfirmationDialog` function
returned.

```tsx
import { Button } from 'primereact/button';
import { DialogButtons, useConfirmationDialog } from '@cratis/applications.react/dialogs';

export const Feature = () => {
    const [showConfirmationDialog] = useConfirmationDialog();

    return (
        <div>
            <Button onClick={() => showConfirmationDialog('The title', 'The message', DialogButtons.YesNo) }/>
        </div>
    )
}
```

The code uses the `showConfirmationDialog` function to pass the definition of the dialog, rather than using the
hook.

In order for there to be a confirmation dialog at all, you need to define how it looks like and also configure it
for your app.

### Defining the Confirmation Dialog

You define a confirmation dialog on the application level. It is then the dialog that will be used across your entire application.

The anatomy of any dialog is that it uses the `useDialogContext()` in the dialog itself to get what context it is in.
With the context you get access to the actual request payload and the method to call when the dialog should close; `closeDialog`.

Below is an example using [Prime React](http://primereact.org) to create a confirmation dialog supporting the different button types.

```tsx
import { Dialog } from 'primereact/dialog';
import { DialogButtons, DialogResult, ConfirmationDialogRequest, useDialogContext } from '@cratis/applications.react/dialogs';
import { Button } from 'primereact/button';

export const ConfirmationDialog = () => {
    const { request, closeDialog } = useDialogContext<ConfirmationDialogRequest>();

    const headerElement = (
        <div className="inline-flex align-items-center justify-content-center gap-2">
            <span className="font-bold white-space-nowrap">{request.title}</span>
        </div>
    );

    const okFooter = (
        <>
            {/* Hook up buttons with function to close the dialog with expected DialogResult */}
            <Button label="Ok" icon="pi pi-check" onClick={() => closeDialog(DialogResult.Ok)} autoFocus />
        </>
    );

    const okCancelFooter = (
        <>
            {/* Hook up buttons with function to close the dialog with expected DialogResult */}
            <Button label="Ok" icon="pi pi-check" onClick={() => closeDialog(DialogResult.Ok)} autoFocus />
            <Button label="Cancel" icon="pi pi-times" severity='secondary' onClick={() => closeDialog(DialogResult.Cancelled)} />
        </>
    );

    const yesNoFooter = (
        <>
            {/* Hook up buttons with function to close the dialog with expected DialogResult */}
            <Button label="Yes" icon="pi pi-check" onClick={() => closeDialog(DialogResult.Yes)} autoFocus />
            <Button label="No" icon="pi pi-times" severity='secondary' onClick={() => closeDialog(DialogResult.No)} />
        </>
    );

    const yesNoCancelFooter = (
        <>
            {/* Hook up buttons with function to close the dialog with expected DialogResult */}
            <Button label="Yes" icon="pi pi-check" onClick={() => closeDialog(DialogResult.Yes)} autoFocus />
            <Button label="No" icon="pi pi-times" severity='secondary' onClick={() => closeDialog(DialogResult.No)} />
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
            {/* On hide we call the closeDialog with cancelled */}
            <Dialog header={headerElement} modal footer={footer} onHide={() => closeDialog(DialogResult.Cancelled)} visible={true}>
                <p className="m-0">
                    {request.message}
                </p>
            </Dialog>
        </>
    );
};
```

The code above uses the `useDialogContext()` with the `ConfirmationDialogRequest` and `DialogResult` as the types expected from
the request and response type. Within the rendering of the component you'll notice that buttons are hooked up to close the
dialog with the expected `DialogResult`. Once a button is called, it closes the dialog with a response that will be passed
onto the `Promise` created within the `IDialogs` service.

> Note: It is possible to take the **request** as props instead of using it from the dialog context
> `export const ConfirmationDialog = (props: ConfirmationDialogRequest) ....`

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

There is a custom React hook for showing a busy indicator.

```tsx
import { Button } from 'primereact/button';
import {useBusyIndicator } from '@cratis/applications.react/dialogs';

export const Feature = () => {
    const [showBusyIndicator, closeBusyIndicator] = useBusyIndicator('The title', 'The message');

    return (
        <div>
            <Button onClick={() => {
                showBusyIndicator();

                setTimeout(() => {
                    closeBusyIndicator();
                }, 1000);
            }} />
        </div>
    )
}
```

The code uses the `useBusyIndicator` hook that returns a function for showing the dialog.
It passes the definition of the dialog as parameters to the hook, setting the title and the message  to show.

Alternatively to defining the dialog on the hook level, you can also pass it along on the `useBusyIndicator` function
returned.

```tsx
import { Button } from 'primereact/button';
import {useBusyIndicator } from '@cratis/applications.react/dialogs';

export const Feature = () => {
    const [showBusyIndicator, closeBusyIndicator] = useBusyIndicator();

    return (
        <div>
            <Button onClick={() => {
                showBusyIndicator('The title', 'The message');

                setTimeout(() => {
                    closeBusyIndicator();
                }, 1000);
            }} />
        </div>
    )
}
```

The code uses the `showBusyIndicator` function to pass the definition of the dialog, rather than using the
hook.

In order for there to be a busy indicator dialog at all, you need to define how it looks like and also configure it
for your app.

### Defining the Busy Indicator Dialog

As with the confirmation dialog, you define a busy indicator dialog on the application level. It is then the dialog that will be used across your entire application.

The anatomy of any dialog is that it uses the `useDialogContext()` in the dialog itself to get what context it is in.
With the context you get access to the actual request payload and the method to call when the dialog should close, or the `closeDialog`
as it is called.

Below is an example using [Prime React](http://primereact.org) to create a confirmation dialog supporting the different button types.

```tsx
import { Dialog } from 'primereact/dialog';
import { BusyIndicatorDialogRequest, useDialogContext } from '@cratis/applications.react/dialogs';
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
the request and response type. For this implementation, it shows a spinner.

> Note: It is possible to take the **request** as props instead of using it from the dialog context.
> `export const BusyIndicatorDialog = (props: BusyIndicatorDialogRequest) ....`

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

Creating a custom dialog is basically just creating a component that represents the dialog and then use
the `useDialog()` hook in the component you want to use the component.

For **closing** the dialog you do however need access to a function that will close it and pass any
result back to the consumer. This can either be done through a **props** definition for the dialog
component that holds the function or you can get to it by getting the **dialog context**.

The following code creates a custom dialog component using [Prime React](http://primereact.org).

```tsx
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { useDialogContext, DialogResult } from '@cratis/applications.react/dialogs';

export const CustomDialog = () => {
    const { request, closeDialog } = useDialogContext();

    return (
        <Dialog header="My custom dialog" visible={true} onHide={() => closeDialog(DialogResult.Cancelled, 'Did not do it..')}>
            <h2>Dialog</h2>
            {request.content}
            <br />
            <Button onClick={() => closeDialog(DialogResult.Ok, 'Done done done...')}>We're done</Button>
        </Dialog>
    );
};
```

The dialog component is a standard component with the exception of the use of the `useDialogContext()` hook.
This hook gives you the current details about the dialog and a function delegate for closing the dialog.

With the `closeDialog` delegate you get to signal what the result of the dialog was and if there is
an expected response, you can also communicate this back to the consumer.

> Note: The `visible` property is always true. This is by design as the logic for showing it or not
> is handled by the `useDialog` hook itself.

For consuming the dialog, we use the `useDialog()` hook. The following code shows an example of this.

```tsx
import { CustomDialog } from './CustomDialog';
import { Button } from 'primereact/button';

export const Feature = () => {
    const [CustomDialogWrapper, showCustomDialog] = useDialog<string>(CustomDialog);

    return (
        <>
            <Button onClick={async () => {
                const [result, response] = await showCustomDialog();
                if( result == DialogResult.Ok ) {
                    console.log('It was ok');
                }
                console.log(response);
            }}>Show dialog</Button>
            <CustomDialogWrapper />
        </>
    )
};
```

The `useDialog()` hook returns a tuple with the wrapper of the dialog and a function for showing the
dialog. Within the component you see that the `showCustomDialog` function is an async function that
will return the `DialogResult` and any response from the dialog.
