// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogContext, DialogContextContent, useDialogContext } from './DialogContext';
import { DialogProps } from './DialogProps';
import { DialogResponse } from './DialogResponse';
import { DialogResult } from './DialogResult';
import { useCallback, useRef, useState, ComponentType, FC, useMemo, use } from 'react';
import { WrappedDialogComponent } from './WrappedDialogComponent';
import { ActualDialogProps } from './ActualDialogProps';
import { ShowDialog } from './ShowDialog';

/**
 * Use a dialog component in you application. This hook manages the visibility and properties of the dialog.
 * @param DialogComponent The dialog component to use.
 * @returns A tuple containing the wrapped dialog component and a function to show the dialog. 
 * The wrapped dialog component will receive the properties passed to it, excluding the `closeDialog` property.
 */
export function useDialog<TResponse = {}, TProps extends DialogProps<TResponse> = { closeDialog: () => void }>(
    DialogComponent: ComponentType<TProps>
): [WrappedDialogComponent<TProps>, ShowDialog<TProps, TResponse>] {

    const [visible, setVisible] = useState(false);
    const [dialogProps, setDialogProps] = useState<ActualDialogProps<TProps> | undefined>();
    const resolverRef = useRef<((value: DialogResponse<TResponse>) => void) | undefined>(undefined);
    const dialogContext = useDialogContext<TProps, TResponse>();

    const showDialog = useCallback((p?: ActualDialogProps<TProps>) => {
        setDialogProps(p);
        setVisible(true);
        return new Promise<DialogResponse<TResponse>>((resolve) => {
            resolverRef.current = resolve;
        });
    }, []);

    const closeDialog = useCallback((result: DialogResult, value?: TResponse) => {
        setVisible(false);
        resolverRef.current?.([result, value]);
    }, []);

    const dialogContextValue = useRef<DialogContextContent<TProps, TResponse>>(undefined!);
    dialogContextValue.current = useMemo(() => {
        return new DialogContextContent(dialogContext?.request, closeDialog);
    }, []);

    const DialogWrapper: WrappedDialogComponent<TProps> = (extraProps) => {
        return visible ? (
            <>
                {console.log('useDialog Context3 = ', dialogContextValue.current)},
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
