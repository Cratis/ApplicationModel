// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useEffect, useMemo, useRef, useState, ComponentType, FC } from 'react';
import { Constructor } from '@cratis/fundamentals';
import { DialogContext, DialogContextContent, CloseDialog, DialogResult } from '@cratis/applications.react/dialogs';
import { useDialogMediator } from './DialogMediator';

export interface DialogProps<TResponse> {
    closeDialog?: CloseDialog<TResponse>
}

type ActualDialogProps<T> = Omit<T, 'closeDialog'>;

/**
 * Use a dialog request for showing a dialog, similar to useDialog.
 * @param requestType Type of request to use that represents a request that will be made by your view model.
 * @param DialogComponent The dialog component to render.
 * @returns A tuple with a component to use for rendering the dialog.
 */
export function useDialogRequest<TRequest extends object, TResponse, TProps extends DialogProps<TResponse> = {}>(
    requestType: Constructor<TRequest>,
    DialogComponent: ComponentType<TProps>
): [FC<ActualDialogProps<TProps>>] {
    const mediator = useDialogMediator();
    const [visible, setVisible] = useState(false);
    const [dialogProps, setDialogProps] = useState<ActualDialogProps<TProps> | undefined>();
    const closeDialogRef = useRef<CloseDialog<TResponse> | undefined>(undefined);
    const dialogContextValue = useRef<DialogContextContent<TRequest, TResponse>>(undefined!);

    const requester = (request: TRequest, closeDialog: CloseDialog<TResponse>) => {
        dialogContextValue.current = new DialogContextContent(request, closeDialog);
        closeDialogRef.current = closeDialog;
        setVisible(true);
    };

    const closeDialog = (result: DialogResult, response?: TResponse) => {
        closeDialogRef.current?.(result, response);
        closeDialogRef.current = undefined;
        setVisible(false);
    };

    dialogContextValue.current = useMemo(() => {
        return new DialogContextContent(undefined!, closeDialog);
    }, []);

    useEffect(() => {
        mediator.subscribe(requestType, requester, closeDialog);
    }, []);

    const DialogWrapper: FC<ActualDialogProps<TProps>> = (extraProps) => {
        return visible ? (
            <DialogContext.Provider value={dialogContextValue.current as unknown as DialogContextContent<object, object>}>
                <DialogComponent
                    {...(dialogProps as TProps)}
                    {...extraProps}
                    closeDialog={closeDialog}
                />
            </DialogContext.Provider>
        ) : null;
    };

    return [DialogWrapper];
}
