// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import sinon from 'sinon';
import { Command, CommandValidator, CommandResult } from '@cratis/applications/commands';
import { PropertyDescriptor } from '@cratis/applications/reflection';

export interface FakeCommandContent {
    someProperty?: string;
    anotherProperty?: number;
}

export class FakeCommand extends Command<FakeCommandContent> {
    readonly route = '/api/fake-command';
    readonly validation = {} as CommandValidator;
    readonly propertyDescriptors: PropertyDescriptor[] = [];

    someProperty?: string;
    anotherProperty?: number;

    get requestParameters(): string[] {
        return [];
    }

    get properties(): string[] {
        return ['someProperty', 'anotherProperty'];
    }

    constructor() {
        super(Object, false);
    }
}
