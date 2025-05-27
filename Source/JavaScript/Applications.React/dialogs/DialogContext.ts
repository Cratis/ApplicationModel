// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import * as React from 'react';
import { useContext } from 'react';
import { CloseDialog } from './CloseDialog';

export interface IDialogContext<TRequest extends object, TResponse> {
    request: TRequest;
    closeDialog: CloseDialog<TResponse>;
}

export const DialogContext = React.createContext<IDialogContext<object, object>>(undefined!);

export const useDialogContext = <TResponse = {}, TRequest extends object = {}>(): IDialogContext<TRequest, TResponse> => {
    return useContext(DialogContext) as unknown as IDialogContext<TRequest, TResponse>;
};
