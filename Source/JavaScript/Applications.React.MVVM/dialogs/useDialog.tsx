// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useEffect, ComponentType, FC, useCallback } from 'react';
import { Constructor } from '@cratis/fundamentals';
import { DialogResult, DialogProps, DialogResponse, ShowDialog, WrappedDialogComponent, useDialog as useDialogBase, useDialogContext } from '@cratis/applications.react/dialogs';
import { useDialogMediator } from './DialogMediator';

/**
 * Use a dialog request for showing a dialog, similar to useDialog.
 * @param requestType Type of request to use that represents a request that will be made by your view model.
 * @param DialogComponent The dialog component to render.
 * @returns A tuple with a component to use for rendering the dialog.
 */
export function useDialog<TRequest extends object, TProps extends DialogProps<TResponse> = { closeDialog: () => void }, TResponse = {}>(
    requestType: Constructor<TRequest>,
    DialogComponent: ComponentType<TProps>
): [WrappedDialogComponent<TProps>, ShowDialog<TProps, TResponse>] {
    const mediator = useDialogMediator();

    const [DialogWrapper, showDialog] = useDialogBase<TResponse, TProps>(DialogComponent);
    const dialogContext = useDialogContext<TRequest, TResponse>();

    const closeDialog = useCallback((result: DialogResult, value?: TResponse) => {
        dialogContext.closeDialog(result, value as TResponse);
    }, []);

    useEffect(() => {
        mediator.subscribe(requestType, (request) => showDialog(request as unknown as TProps), closeDialog);
    }, []);

    return [DialogWrapper, showDialog];
}
