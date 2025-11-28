// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { QueryFor, QueryResult, Paging, Sorting } from '@cratis/applications/queries';

export interface FakeQueryResult {
    id: string;
    name: string;
}

export class FakeQuery extends QueryFor<FakeQueryResult[]> {
    readonly route = '/api/fake-query';

    get requiredRequestParameters(): string[] {
        return [];
    }

    defaultValue: FakeQueryResult[] = [];

    constructor() {
        super(Object, true);
    }
}
