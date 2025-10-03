// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestObservableQuery } from './given/TestQueries';
import { expect } from 'chai';

describe('when setting api base path', () => {
    let query: TestObservableQuery;

    beforeEach(() => {
        query = new TestObservableQuery();
    });

    it('should not throw when setting api base path', () => {
        expect(() => query.setApiBasePath('/api/v1')).to.not.throw();
    });
});