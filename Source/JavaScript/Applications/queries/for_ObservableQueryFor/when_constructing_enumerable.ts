// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestEnumerableQuery } from './given/TestQueries';
import { Sorting } from '../Sorting';
import { Paging } from '../Paging';
import { expect } from 'chai';

describe('when constructing enumerable query', () => {
    let query: TestEnumerableQuery;

    beforeEach(() => {
        query = new TestEnumerableQuery();
    });

    it('should set sorting to none', () => expect(query.sorting).to.equal(Sorting.none));
    it('should set paging to no paging', () => expect(query.paging).to.equal(Paging.noPaging));
    it('should set model type to String', () => expect(query.modelType).to.equal(String));
    it('should set enumerable to true', () => expect(query.enumerable).to.be.true);
    it('should have default required request parameters', () => expect(query.requiredRequestParameters).to.deep.equal(['category']));
    it('should have default route', () => expect(query.route).to.equal('/api/items/{category}'));
    it('should have default value as empty array', () => expect(query.defaultValue).to.deep.equal([]));
});