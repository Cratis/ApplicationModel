// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import sinon from 'sinon';
import { CommandResult, ICommand, PropertyChanged } from '@cratis/arc/commands';
import { PropertyDescriptor } from '@cratis/arc/reflection';

/* eslint-disable */

export class FakeCommand implements ICommand {
    route = '';
    propertyDescriptors: PropertyDescriptor[] = [];
    private _hasChanges: boolean;

    constructor(hasChanges: boolean) {
        this._hasChanges = hasChanges;
        this.execute = sinon.fake(() => {
            this._hasChanges = false;
            return new Promise<CommandResult>(resolve => {
                resolve(CommandResult.empty);
            });
        });
        this.setInitialValues = sinon.stub();
        this.setInitialValues = sinon.stub();
        this.propertyChanged = sinon.stub();
        this.onPropertyChanged = sinon.stub();
        this.revertChanges = sinon.fake(() => {
            this._hasChanges = false;
        });
    }

    get hasChanges() {
        return this._hasChanges;
    }

    revertChanges(): void {
        throw new Error('Method not implemented.');
    }

    execute(): Promise<CommandResult> {
        throw new Error('Method not implemented.');
    }
    setInitialValues(values: {}): void {
        throw new Error('Method not implemented.');
    }
    setInitialValuesFromCurrentValues(): void {
        throw new Error('Method not implemented.');
    }
    propertyChanged(property: string): void {
        throw new Error('Method not implemented.');
    }
    onPropertyChanged(callback: PropertyChanged, thisArg: any): void {
        throw new Error('Method not implemented.');
    }

    setMicroservice(microservice: string) {
        throw new Error('Method not implemented.');
    }
}
