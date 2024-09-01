// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResult } from '@cratis/applications.react/dialogs';
import { DialogButtons } from './DialogButtons';
import { IDialogs } from './IDialogs';
import { StandardDialogRequest } from './StandardDialogRequest';
import { IDialogMediatorHandler } from './IDialogMediatorHandler';

/**
 * Represents an implementation of {@link IDialogs}.
 */
export class Dialogs extends IDialogs {

    /**
     * Initializes a new instance of the {@link Dialogs} class.
     */
    constructor(private readonly _dialogMediatorContext: IDialogMediatorHandler) {
        super();
    }

    /** @inheritdoc */
    show<TInput extends {}, TOutput>(input: TInput): Promise<TOutput> {
        return this._dialogMediatorContext.show<TInput, TOutput>(input);
    }

    /** @inheritdoc */
    showStandard(title: string, message: string, buttons: DialogButtons): Promise<DialogResult> {
        return this.show<StandardDialogRequest, DialogResult>(new StandardDialogRequest(title, message, buttons));
    }
}