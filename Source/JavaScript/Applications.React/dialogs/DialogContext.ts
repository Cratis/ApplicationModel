// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import * as React from 'react';
import { useContext } from 'react';
import { CloseDialog } from './CloseDialog';

export class DialogContextContent<TRequest extends object, TResponse> {
    constructor(readonly request: TRequest, readonly closeDialog: CloseDialog<TResponse>) {
    }
}

export const DialogContext = React.createContext<DialogContextContent<object, object>>(undefined!);

export const useDialogContext = <TResponse = {}, TRequest extends object = {}>(): DialogContextContent<TRequest, TResponse> => {
    return useContext(DialogContext) as unknown as DialogContextContent<TRequest, TResponse>;
};
