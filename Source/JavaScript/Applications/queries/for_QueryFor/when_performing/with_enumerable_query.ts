// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from '../given/a_query_for';
import { given } from '../../../given';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { QueryResult } from '../../QueryResult';

describe('with enumerable query', given(a_query_for, context => {
    let result: QueryResult<string[]>;
    let fetchStub: sinon.SinonStub;
    const mockResponse = {
        data: ['item1', 'item2'],
        isSuccess: true,
        isAuthorized: true,
        isValid: true,
        hasExceptions: false,
        validationResults: [],
        exceptionMessages: [],
        exceptionStackTrace: '',
        paging: {
            totalItems: 0,
            totalPages: 0,
            page: 0,
            size: 0
        }
    };

    beforeEach(async () => {
        // Setup fetch mock
        fetchStub = sinon.stub(global, 'fetch');
        fetchStub.resolves({
            json: sinon.stub().resolves(mockResponse),
            ok: true,
            status: 200
        } as unknown as Response);

        context.enumerableQuery.setOrigin('https://api.example.com');

        // Call perform with valid arguments
        result = await context.enumerableQuery.perform({ category: 'test-category' });
    });

    afterEach(() => {
        fetchStub.restore();
    });

    it('should return successful result', () => {
        expect(result.isSuccess).to.be.true;
    });

    it('should return array data', () => {
        expect(result.data).to.deep.equal(['item1', 'item2']);
    });

    it('should call fetch with correct URL', () => {
        expect(fetchStub).to.have.been.calledOnce;
        const call = fetchStub.getCall(0);
        expect(call.args[0].href).to.equal('https://api.example.com/api/items/test-category');
    });
}));