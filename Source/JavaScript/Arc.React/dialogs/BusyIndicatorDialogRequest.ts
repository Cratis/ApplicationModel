// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the request for a busy indicator dialog, typically for spinners.
 */
export class BusyIndicatorDialogRequest {

    /**
     * Initializes a new instance of {@link BusyIndicatorDialogRequest}.
     * @param {String} title The title of the dialog.
     * @param {String} message The message to show in the dialog.
     */
    constructor(
        readonly title: string,
        readonly message: string) {
    }
}
