// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResult } from '@cratis/applications.react/dialogs';
import { DialogButtons } from './DialogButtons';

/**
 * Defines a service for working with dialogs from a view model.
 */
export abstract class IDialogs {
    /**
     * Show a dialog in the context of the current view and view model. This requires the view to host the dialog.
     * @param {*} input The input to pass to the dialog.
     */
    abstract show<TInput extends {}, TOutput>(input: TInput): Promise<TOutput>;

    /**
     * Show a standard confirmation dialog.
     * @param {String} title Title of the dialog.
     * @param {String} message Message to show inside the dialog.
     * @param {DialogButtons} buttons Buttons to have on the dialog
     */
    abstract showConfirmation(title: string, message: string, buttons: DialogButtons): Promise<DialogResult>;
}
