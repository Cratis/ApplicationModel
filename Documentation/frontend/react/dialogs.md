# Dialogs

Cratis Application Model provides a consistent way of working with dialogs independent of component libraries.
The purpose is to promote a way of working with dialogs as separate components and not something you
do inline within another component.

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
