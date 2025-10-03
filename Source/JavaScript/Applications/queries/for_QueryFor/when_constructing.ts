// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from './given/a_query_for';
import { given } from '../../given';
import { Sorting } from '../Sorting';
import { Paging } from '../Paging';
import { Globals } from '../../Globals';
import { expect } from 'chai';

describe('when constructing', given(a_query_for, context => {
    let originalMicroservice: string | undefined;

    beforeEach(() => {
        originalMicroservice = Globals.microservice;
        Globals.microservice = 'test-microservice';
    });

    afterEach(() => {
        if (originalMicroservice !== undefined) {
            Globals.microservice = originalMicroservice;
        }
    });

    it('should set sorting to none', () => expect(context.query.sorting).to.equal(Sorting.none));
    it('should set paging to no paging', () => expect(context.query.paging).to.equal(Paging.noPaging));
    it('should set model type to String', () => expect(context.query.modelType).to.equal(String));
    it('should set enumerable to false', () => expect(context.query.enumerable).to.be.false);
    it('should have default required request parameters', () => expect(context.query.requiredRequestParameters).to.deep.equal(['id']));
    it('should have default route', () => expect(context.query.route).to.equal('/api/test/{id}'));
    it('should have default value as empty string', () => expect(context.query.defaultValue).to.equal(''));
}));