// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from '../given/a_query_for';
import { given } from '../../../given';
import { expect } from 'chai';
import { QueryResult } from '../../QueryResult';

describe('with invalid arguments', given(a_query_for, context => {
    let result: QueryResult<string>;

    beforeEach(async () => {
        // Call perform without required arguments
        result = await context.query.perform();
    });

    it('should return no success result', () => {
        expect(result.isSuccess).to.be.false;
    });

    it('should return default value', () => {
        expect(result.data).to.equal('');
    });

    it('should return result without data', () => {
        // For validation failures, the result doesn't have proper data
        expect(result.data).to.equal('');
        expect(result.isSuccess).to.be.false;
    });
}));