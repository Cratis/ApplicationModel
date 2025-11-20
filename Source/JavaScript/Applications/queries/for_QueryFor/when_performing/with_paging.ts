// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from '../given/a_query_for';
import { given } from '../../../given';

import * as sinon from 'sinon';
import { QueryResult } from '../../QueryResult';
import { Paging } from '../../Paging';

describe('with paging', given(a_query_for, context => {
    let result: QueryResult<string>;
    let fetchStub: sinon.SinonStub;
    const mockResponse = {
        data: 'test-result',
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

        context.query.setOrigin('https://api.example.com');
        context.query.paging = new Paging(2, 10); // page 2, 10 items per page

        // Call perform with valid arguments
        result = await context.query.perform({ id: 'test-id' });
    });

    afterEach(() => {
        fetchStub.restore();
    });

    it('should call fetch with URL including paging parameters', () => {
        fetchStub.should.have.been.calledOnce;
        const call = fetchStub.getCall(0);
        const url = call.args[0].href;
        url.should.include('page=2');
        url.should.include('pageSize=10');
    });

    it('should return successful result', () => {
        result.isSuccess.should.be.true;
    });
}));