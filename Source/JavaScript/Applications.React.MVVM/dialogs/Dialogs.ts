// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResult } from '@cratis/applications.react/dialogs';
import { DialogButtons } from './DialogButtons';
import { IDialogs } from './IDialogs';
import { BusyIndicator } from './BusyIndicator';
import { ConfirmationDialogRequest } from './ConfirmationDialogRequest';
import { IDialogMediatorHandler } from './IDialogMediatorHandler';
import { BusyIndicatorDialogRequest } from './BusyIndicatorDialogRequest';
import { DialogContext } from './useDialogRequest';
import React from 'react';

/**
 * Represents an implementation of {@link IDialogs}.
 */
export class Dialogs extends IDialogs {

    /**
     * Initializes a new instance of the {@link Dialogs} class.
     */
    constructor(private readonly _dialogMediatorHandler: IDialogMediatorHandler) {
        super();
    }

    /** @inheritdoc */
    show<TInput extends {}, TOutput>(input: TInput): Promise<TOutput> {
        return this._dialogMediatorHandler.show<TInput, TOutput>(input);
    }

    /** @inheritdoc */
    showConfirmation(title: string, message: string, buttons: DialogButtons): Promise<DialogResult> {
        return this.show<ConfirmationDialogRequest, DialogResult>(new ConfirmationDialogRequest(title, message, buttons));
    }

    /** @inheritdoc */
    showBusyIndicator(title: string, message: string): BusyIndicator {
        this.show<BusyIndicatorDialogRequest, void>(new BusyIndicatorDialogRequest(title, message));
        const registration = this._dialogMediatorHandler.getRegistration<BusyIndicatorDialogRequest, void>(BusyIndicatorDialogRequest);
        const busyIndicator = new BusyIndicator(registration.resolver);
        return busyIndicator;
    }
}
