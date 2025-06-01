// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResponse, DialogResult, IDialogComponents, ConfirmationDialogRequest, BusyIndicatorDialogRequest, CloseDialog } from '@cratis/applications.react/dialogs';
import { DialogButtons } from '@cratis/applications.react/dialogs/DialogButtons';
import { IDialogs } from './IDialogs';
import { BusyIndicator } from './BusyIndicator';
import { IDialogMediatorHandler } from './IDialogMediatorHandler';

/**
 * Represents an implementation of {@link IDialogs}.
 */
export class Dialogs extends IDialogs {

    /**
     * Initializes a new instance of the {@link Dialogs} class.
     */
    constructor(
        private readonly _dialogMediatorHandler: IDialogMediatorHandler,
        dialogComponents: IDialogComponents) {
        super();

        _dialogMediatorHandler.subscribe(ConfirmationDialogRequest, async (request, resolver) => {
            const [result] = await dialogComponents.showConfirmation(request as ConfirmationDialogRequest);
            resolver(result);
        }, () => { });

        let busyIndicatorResolver: CloseDialog<void>;

        _dialogMediatorHandler.subscribe(BusyIndicatorDialogRequest, (request, resolver) => {
            busyIndicatorResolver = resolver;
            return dialogComponents.showBusyIndicator(request as BusyIndicatorDialogRequest);
        }, () => {
            dialogComponents.closeBusyIndicator(DialogResult.Cancelled);
        });
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
