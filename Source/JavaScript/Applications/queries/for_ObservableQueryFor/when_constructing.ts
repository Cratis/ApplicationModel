// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestObservableQuery } from './given/TestQueries';
import { Sorting } from '../Sorting';
import { Paging } from '../Paging';
import { Globals } from '../../Globals';
import { expect } from 'chai';

describe('when constructing', () => {
    let query: TestObservableQuery;
    let originalMicroservice: string | undefined;

    beforeEach(() => {
        originalMicroservice = Globals.microservice;
        Globals.microservice = 'test-microservice';
        query = new TestObservableQuery();
    });

    afterEach(() => {
        if (originalMicroservice !== undefined) {
            Globals.microservice = originalMicroservice;
        }
    });

    it('should set sorting to none', () => expect(query.sorting).to.equal(Sorting.none));
    it('should set paging to no paging', () => expect(query.paging).to.equal(Paging.noPaging));
    it('should set model type to String', () => expect(query.modelType).to.equal(String));
    it('should set enumerable to false', () => expect(query.enumerable).to.be.false);
    it('should have default required request parameters', () => expect(query.requiredRequestParameters).to.deep.equal(['id']));
    it('should have default route', () => expect(query.route).to.equal('/api/test/{id}'));
    it('should have default value as empty string', () => expect(query.defaultValue).to.equal(''));
});