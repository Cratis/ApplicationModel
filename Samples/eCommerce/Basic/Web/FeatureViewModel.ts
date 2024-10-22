// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { injectable } from 'tsyringe';
import { Cart, ObserveCartForCurrentUser } from './API/Carts';
import { DialogButtons, IDialogs } from '@cratis/applications.react.mvvm/dialogs';
import { CustomDialogRequest } from './CustomDialog';
import { IMessenger } from '@cratis/applications.react.mvvm/messaging';
import { IViewModelDetached } from '@cratis/applications.react.mvvm';
import { IQueryProvider } from '@cratis/applications/queries';
import { IIdentityProvider } from '@cratis/applications/identity';

export class Something {
    constructor(readonly value: string) {
    }
}


@injectable()
export class FeatureViewModel implements IViewModelDetached {
    constructor(
        private readonly _messenger: IMessenger,
        private readonly _dialogs: IDialogs,
        private readonly queryProvider: IQueryProvider,
        private readonly identityProvider: IIdentityProvider) {
        // query.subscribe(async result => {
        //     this.cart = result.data;
        // });

        const query = queryProvider.get(ObserveCartForCurrentUser);

        identityProvider.getCurrent().then(identity => {
            console.log(`Hello ${identity.name}`);
        });

        _messenger.subscribe(Something, something => {
            console.log(`Got something: ${something.value}`);
        });

        setInterval(() => {
            this.counter++;
        }, 1000);
    }

    detached(): void {
    }

    counter: number = 0;
    cart: Cart = new Cart();

    async doStuff() {
        const result = await this._dialogs.show<CustomDialogRequest, string>(new CustomDialogRequest('This is the content to show'));
        console.log(`Result: ${result}`);

        this._messenger.publish(new Something(`Hello: ${result}`));
    }

    async doOtherStuff() {
        const result = await this._dialogs.showConfirmation('Delete?', 'Are you sure you want to delete?', DialogButtons.YesNo);
        console.log(`Result: ${result}`);
    }

    async doHeavyStuff() {
        const busyIndicator = this._dialogs.showBusyIndicator('Doing heavy stuff', 'Please wait');
        setTimeout(() => {
            busyIndicator.close();
        }, 1000);
    }
}