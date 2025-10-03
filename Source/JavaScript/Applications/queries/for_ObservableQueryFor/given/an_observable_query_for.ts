// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestObservableQuery, TestEnumerableQuery } from './TestQueries';

export class an_observable_query_for {
    query: TestObservableQuery;
    enumerableQuery: TestEnumerableQuery;

    constructor() {
        this.query = new TestObservableQuery();
        this.enumerableQuery = new TestEnumerableQuery();
    }
}