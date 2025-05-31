// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useEffect, useMemo, useRef, useState, ComponentType, FC, useCallback } from 'react';
import { Constructor } from '@cratis/fundamentals';
import { DialogContext, DialogContextContent, CloseDialog, DialogResult, DialogProps, DialogResponse, ShowDialog, WrappedDialogComponent, ActualDialogProps } from '@cratis/applications.react/dialogs';
import { useDialogMediator } from './DialogMediator';

/**
 * Use a dialog request for showing a dialog, similar to useDialog.
 * @param requestType Type of request to use that represents a request that will be made by your view model.
 * @param DialogComponent The dialog component to render.
 * @returns A tuple with a component to use for rendering the dialog.
 */
export function useDialogRequest<TRequest extends object, TProps extends DialogProps<TResponse> = { closeDialog: () => void }, TResponse = {}>(
    requestType: Constructor<TRequest>,
    DialogComponent: ComponentType<TProps>
): [WrappedDialogComponent<TProps>, ShowDialog<TProps, TResponse>] {
    const mediator = useDialogMediator();
    const [visible, setVisible] = useState(false);
    const [dialogProps, setDialogProps] = useState<ActualDialogProps<TProps> | undefined>();
    const closeDialogRef = useRef<CloseDialog<TResponse> | undefined>(undefined);
    const [currentRequest, setCurrentRequest] = useState<TRequest | undefined>(undefined);

    const showDialog = useCallback((p?: ActualDialogProps<TProps>) => {
        setDialogProps(p);
        setVisible(true);
        return new Promise<DialogResponse<TResponse>>(() => {
            closeDialogRef.current = closeDialog;
        });
    }, []);

    const requester = useCallback((request: TRequest, closeDialog: CloseDialog<TResponse>) => {
        setCurrentRequest(request);
        closeDialogRef.current = closeDialog;
        setDialogProps(request as ActualDialogProps<TProps>);
        setVisible(true);
    }, []);

    const closeDialog = useCallback((result: DialogResult, value?: TResponse) => {
        closeDialogRef.current?.(result, value);
        closeDialogRef.current = undefined;
        setVisible(false);
    }, []);

    const dialogContextValue = useRef<DialogContextContent<TRequest, TResponse>>(undefined!);
    dialogContextValue.current = useMemo(() => {
        return new DialogContextContent(currentRequest!, closeDialog);
    }, [currentRequest]);

    useEffect(() => {
        mediator.subscribe(requestType, requester, closeDialog);
    }, []);

    const DialogWrapper: WrappedDialogComponent<TProps> = (extraProps) => {
        return visible ? (
            <>
                {console.log(dialogContextValue.current)},
                <DialogContext.Provider value={dialogContextValue.current as unknown as DialogContextContent<object, object>}>
                    <DialogComponent
                        {...(dialogProps as TProps)}
                        {...extraProps}
                        closeDialog={closeDialog} />
                </DialogContext.Provider>
            </>
        ) : null;
    };

    return [DialogWrapper, showDialog];
}
