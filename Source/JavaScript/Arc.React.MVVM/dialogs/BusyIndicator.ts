// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { CloseDialog, DialogResult } from '@cratis/arc.react/dialogs';

/**
 * Represents a busy indicator dialog.
 */
export class BusyIndicator {

    /**
     * Initializes a new instance of {@link BusyIndicator}.
     * @param {CloseDialog<void>} _closeDialog The dialog resolver to use for closing the dialog.
     */
    constructor(private readonly _closeDialog: CloseDialog<void>) {
    }

    /**
     * Close the busy indicator dialog.
     */
    close() {
        this._closeDialog(DialogResult.None);
    }
}
