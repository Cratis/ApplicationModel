// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Constructor } from '@cratis/fundamentals';
import { IDialogMediatorHandler } from './IDialogMediatorHandler';
import { DialogRegistration, DialogRequest, DialogResolver } from './DialogRegistration';

/**
 * Represents an implementation of {@link IDialogMediatorHandler}
 */
export class DialogMediatorHandler extends IDialogMediatorHandler {
    private _registrations: WeakMap<Constructor, DialogRegistration<object, object>> = new WeakMap();

    /**
     * Initializes a new instance of {@link DialogMediatorHandler}
     * @param {IDialogMediatorHandler} _parent Optional parent handler.
     */
    constructor(readonly _parent: IDialogMediatorHandler | null = null) {
        super();
    }

    /** @inheritdoc */
    subscribe<TRequest extends object, TResponse>(requestType: Constructor<TRequest>, requester: DialogRequest<TRequest, TResponse>, resolver: DialogResolver<TResponse>): void {
        this._registrations.set(
            requestType,
            new DialogRegistration<TRequest, TResponse>(requester, resolver) as unknown as DialogRegistration<object, object>);
    }

    /** @inheritdoc */
    hasSubscriber<TRequest extends object>(requestType: Constructor<TRequest>): boolean {
        return this._registrations.has(requestType);
    }

    /** @inheritdoc */
    show<TRequest extends object, TResponse>(request: TRequest): Promise<TResponse> {
        if (!this.hasSubscriber(request.constructor as Constructor)) {
            if (this._parent) {
                return this._parent.show(request);
            }

            return Promise.reject('No registration found for request');
        }

        const promise = new Promise<TResponse>((resolve) => {
            const registration = this._registrations.get(request.constructor as Constructor)!;
            registration.requester(request, resolve as unknown as DialogResolver<object>);
        });

        return promise;
    }

    /** @inheritdoc */
    getRegistration<TRequest extends object, TResponse>(requestType: Constructor<TRequest>): DialogRegistration<TRequest, TResponse> {
        if (!this.hasSubscriber(requestType)) {
            if (this._parent) {
                return this._parent.getRegistration(requestType);
            }

            throw new Error('No registration found for request');
        }

        return this._registrations.get(requestType)! as unknown as DialogRegistration<TRequest, TResponse>;
    }
}
