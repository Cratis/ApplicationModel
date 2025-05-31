// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResult } from '@cratis/applications.react/dialogs';
import { DialogButtons } from './DialogButtons';
import { IDialogs } from './IDialogs';
import { BusyIndicator } from './BusyIndicator';
import { ConfirmationDialogRequest } from './ConfirmationDialogRequest';
import { IDialogMediatorHandler } from './IDialogMediatorHandler';
import { BusyIndicatorDialogRequest } from './BusyIndicatorDialogRequest';
import { DialogResponse } from '../../Applications.React/dist/cjs/dialogs/DialogResponse';

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
    show<TRequest extends object, TResponse = {}>(request: TRequest): Promise<DialogResponse<TResponse>> {
        return this._dialogMediatorHandler.show<TRequest, TResponse>(request);
    }

    /** @inheritdoc */
    async showConfirmation(title: string, message: string, buttons: DialogButtons): Promise<DialogResult> {
        const [result] = await this.show<ConfirmationDialogRequest>(new ConfirmationDialogRequest(title, message, buttons));
        return result;
    }

    /** @inheritdoc */
    showBusyIndicator(title: string, message: string): BusyIndicator {
        this.show<BusyIndicatorDialogRequest, void>(new BusyIndicatorDialogRequest(title, message));
        const registration = this._dialogMediatorHandler.getRegistration<BusyIndicatorDialogRequest, void>(BusyIndicatorDialogRequest);
        const busyIndicator = new BusyIndicator(registration.resolver);
        return busyIndicator;
    }
}
