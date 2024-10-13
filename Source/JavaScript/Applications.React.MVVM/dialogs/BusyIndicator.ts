// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResolver } from './DialogRegistration';

/**
 * Represents a busy indicator dialog.
 */
export class BusyIndicator {

    /**
     * Initializes a new instance of {@link BusyIndicator}.
     * @param {DialogResolver<void>} _dialogResolver The dialog resolver to use for closing the dialog.
     */
    constructor(private readonly _dialogResolver: DialogResolver<void>) {
    }

    /**
     * Close the busy indicator dialog.
     */
    close() {
        this._dialogResolver();
    }
}
