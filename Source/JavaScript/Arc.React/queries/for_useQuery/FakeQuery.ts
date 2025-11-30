// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { QueryFor } from '@cratis/arc/queries';
import { ParameterDescriptor } from '@cratis/arc/reflection';

export interface FakeQueryResult {
    id: string;
    name: string;
}

export class FakeQuery extends QueryFor<FakeQueryResult[]> {
    readonly route = '/api/fake-query';
    readonly parameterDescriptors: ParameterDescriptor[] = [];

    get requiredRequestParameters(): string[] {
        return [];
    }

    defaultValue: FakeQueryResult[] = [];

    constructor() {
        super(Object, true);
    }
}
