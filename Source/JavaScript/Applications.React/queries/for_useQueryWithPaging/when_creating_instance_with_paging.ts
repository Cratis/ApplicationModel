// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { render } from '@testing-library/react';
import sinon from 'sinon';
import { useQueryWithPaging } from '../useQuery';
import { FakeQuery } from '../for_useQuery/FakeQuery';
import { ApplicationModelContext, ApplicationModelConfiguration } from '../../ApplicationModelContext';
import { Paging } from '@cratis/applications/queries';

describe('when creating instance with paging', () => {
    let fetchStub: sinon.SinonStub;

    beforeEach(() => {
        fetchStub = sinon.stub(global, 'fetch').resolves({
            json: async () => ({ data: [], isSuccess: true, isAuthorized: true, isValid: true, hasExceptions: false, validationResults: [], exceptionMessages: [], exceptionStackTrace: '' })
        } as Response);
    });

    afterEach(() => {
        fetchStub.restore();
    });

    const config: ApplicationModelConfiguration = {
        microservice: 'test-microservice',
        apiBasePath: '/api',
        origin: 'https://example.com',
        httpHeadersCallback: () => ({ 'X-Custom-Header': 'custom-value' })
    };

    let queryInstance: FakeQuery | null = null;
    const paging = new Paging(1, 10);
    
    const SpyQuery = class extends FakeQuery {
        constructor() {
            super();
            queryInstance = this;
        }
    };

    render(
        React.createElement(
            ApplicationModelContext.Provider,
            { value: config },
            React.createElement(() => {
                useQueryWithPaging(SpyQuery, paging);
                return React.createElement('div', null, 'Test');
            })
        )
    );

    it('should set paging on the query', () => queryInstance!.paging.should.equal(paging));
    it('should set microservice from context', () => queryInstance!['_microservice'].should.equal('test-microservice'));
    it('should set api base path from context', () => queryInstance!['_apiBasePath'].should.equal('/api'));
    it('should set origin from context', () => queryInstance!['_origin'].should.equal('https://example.com'));
    it('should set http headers callback from context', () => {
        const headers = queryInstance!['_httpHeadersCallback']();
        headers.should.deep.equal({ 'X-Custom-Header': 'custom-value' });
    });
});
