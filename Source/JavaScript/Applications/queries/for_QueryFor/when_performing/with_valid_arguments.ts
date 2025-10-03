// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from '../given/a_query_for';
import { given } from '../../../given';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { QueryResult } from '../../QueryResult';

describe('with valid arguments', given(a_query_for, context => {
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
        context.query.setApiBasePath('/api/v1');
        context.query.setMicroservice('test-service');

        // Call perform with valid arguments
        result = await context.query.perform({ id: 'test-id' });
    });

    afterEach(() => {
        fetchStub.restore();
    });

    it('should return successful result', () => {
        expect(result.isSuccess).to.be.true;
    });

    it('should return correct data', () => {
        expect(result.data).to.equal('test-result');
    });

    it('should have data', () => {
        expect(result.hasData).to.be.true;
    });

    it('should call fetch with correct URL', () => {
        expect(fetchStub).to.have.been.calledOnce;
        const call = fetchStub.getCall(0);
        expect(call.args[0].href).to.equal('https://api.example.com/api/v1/api/test/test-id');
    });

    it('should call fetch with correct headers', () => {
        const call = fetchStub.getCall(0);
        const options = call.args[1];
        expect(options.headers['Accept']).to.equal('application/json');
        expect(options.headers['Content-Type']).to.equal('application/json');
        expect(options.headers['x-cratis-microservice']).to.equal('test-service');
    });

    it('should call fetch with GET method', () => {
        const call = fetchStub.getCall(0);
        const options = call.args[1];
        expect(options.method).to.equal('GET');
    });

    it('should set abort controller signal', () => {
        const call = fetchStub.getCall(0);
        const options = call.args[1];
        expect(options.signal).to.not.be.undefined;
    });
}));