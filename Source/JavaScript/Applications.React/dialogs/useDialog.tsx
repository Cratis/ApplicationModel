// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogContext, DialogContextContent } from './DialogContext';
import { DialogResponse } from './DialogResponse';
import { DialogResult } from './DialogResult';
import { useCallback, useRef, useState, ComponentType, FC, useMemo, useContext } from 'react';
import { ShowDialog } from './ShowDialog';
import { DialogComponentsContext, IDialogComponents } from './DialogComponents';

/**
 * Use a dialog component in you application. This hook manages the visibility and properties of the dialog.
 * @param DialogComponent The dialog component to use.
 * @returns A tuple containing the wrapped dialog component and a function to show the dialog. 
 * The wrapped dialog component will receive the properties passed to it, excluding the `closeDialog` property.
 */
export function useDialog<TResponse = {}, TProps = {}>(
    DialogComponent: ComponentType<TProps>
): [FC<TProps>, ShowDialog<TProps, TResponse>, DialogContextContent<TProps, TResponse>] {

    const [visible, setVisible] = useState(false);
    const [dialogProps, setDialogProps] = useState<TProps | undefined>();
    const resolverRef = useRef<((value: DialogResponse<TResponse>) => void) | undefined>(undefined);
    const dialogComponents = useContext<IDialogComponents>(DialogComponentsContext);

    const showDialog = useCallback((p?: TProps) => {
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
        return new DialogContextContent(dialogProps!, closeDialog);
    }, [dialogProps]);

    const DialogWrapper: FC<TProps> = (extraProps) => {
        return visible ? (
            <DialogContext.Provider value={dialogContextValue.current as unknown as DialogContextContent<object, object>}>
                <DialogComponent
                    {...extraProps}
                    {...(dialogProps as TProps)}
                    closeDialog={closeDialog} />
            </DialogContext.Provider>
        ) : null;
    };

    return [DialogWrapper, showDialog, dialogContextValue.current];
}
