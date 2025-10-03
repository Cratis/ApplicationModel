// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestQueryFor, TestEnumerableQueryFor, TestQueryForWithoutRequiredParams } from './TestQueries';

export class a_query_for {
    query: TestQueryFor;
    enumerableQuery: TestEnumerableQueryFor;
    queryWithoutParams: TestQueryForWithoutRequiredParams;

    constructor() {
        this.query = new TestQueryFor();
        this.enumerableQuery = new TestEnumerableQueryFor();
        this.queryWithoutParams = new TestQueryForWithoutRequiredParams();
    }
}