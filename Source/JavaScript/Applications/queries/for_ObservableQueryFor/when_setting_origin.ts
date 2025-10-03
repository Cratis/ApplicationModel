// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestObservableQuery } from './given/TestQueries';
import { expect } from 'chai';

describe('when setting origin', () => {
    let query: TestObservableQuery;

    beforeEach(() => {
        query = new TestObservableQuery();
    });

    it('should not throw when setting origin', () => {
        expect(() => query.setOrigin('https://example.com')).to.not.throw();
    });
});