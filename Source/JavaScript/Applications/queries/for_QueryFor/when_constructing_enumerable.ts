// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from './given/a_query_for';
import { given } from '../../given';
import { Sorting } from '../Sorting';
import { Paging } from '../Paging';
import { expect } from 'chai';

describe('when constructing enumerable', given(a_query_for, context => {
    it('should set sorting to none', () => expect(context.enumerableQuery.sorting).to.equal(Sorting.none));
    it('should set paging to no paging', () => expect(context.enumerableQuery.paging).to.equal(Paging.noPaging));
    it('should set model type to String', () => expect(context.enumerableQuery.modelType).to.equal(String));
    it('should set enumerable to true', () => expect(context.enumerableQuery.enumerable).to.be.true);
    it('should have default required request parameters', () => expect(context.enumerableQuery.requiredRequestParameters).to.deep.equal(['category']));
    it('should have default route', () => expect(context.enumerableQuery.route).to.equal('/api/items/{category}'));
    it('should have default value as empty array', () => expect(context.enumerableQuery.defaultValue).to.deep.equal([]));
}));