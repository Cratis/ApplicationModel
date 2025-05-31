// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { CloseDialog } from '@cratis/applications.react/dialogs';

/**
 * Represents the delegate for a dialog request.
 */
export type DialogRequest<TRequest extends object, TResponse> = (request: TRequest, closeDialog: CloseDialog<TResponse>) => void;

/**
 * Represents the registration of a dialog.
 */
export class DialogRegistration<TRequest extends object, TResponse> {

    /**
     * Initializes a new instance of {@link DialogRegistration}.
     * @param requester The requester for the dialog.
     * @param resolver The resolver for the dialog.
     */
    constructor(
        readonly requester: DialogRequest<TRequest, TResponse>,
        readonly resolver: CloseDialog<TResponse>) {
    }
}
