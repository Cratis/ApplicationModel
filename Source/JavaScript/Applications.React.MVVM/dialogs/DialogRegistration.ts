// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the delegate when a dialog is resolved (closed with a response).
 */
export type DialogResolver<TResponse> = (response: TResponse) => void;

/**
 * Represents the delegate for a dialog request.
 */
export type DialogRequest<TRequest extends {}, TResponse> = (request: TRequest, resolve: DialogResolver<TResponse>) => void;

/**
 * Represents the registration of a dialog.
 */
export class DialogRegistration<TRequest extends {}, TResponse> {

    /**
     * Initializes a new instance of {@link DialogRegistration}.
     * @param requester The requester for the dialog.
     * @param resolver The resolver for the dialog.
     */
    constructor(
        readonly requester: DialogRequest<TRequest, TResponse>,
        readonly resolver: DialogResolver<TResponse>) {
    }
}
