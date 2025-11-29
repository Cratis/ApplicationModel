// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { render } from '@testing-library/react';
import sinon from 'sinon';
import { useQueryWithPaging } from '../useQuery';
import { FakeQuery } from '../for_useQuery/FakeQuery';
import { ApplicationModelContext, ApplicationModelConfiguration } from '../../ApplicationModelContext';
import { Paging } from '@cratis/applications/queries';

/* eslint-disable @typescript-eslint/no-explicit-any */

describe('when creating instance with paging', () => {
    let fetchStub: sinon.SinonStub;
    let queryInstance: FakeQuery | null = null;

    const captureInstance = (instance: FakeQuery) => {
        queryInstance = instance;
    };

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

    const paging = new Paging(1, 10);
    
    class SpyQuery extends FakeQuery {
        constructor() {
            super();
            captureInstance(this);
        }
    }

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
    it('should set microservice from context', () => ((queryInstance as any)._microservice).should.equal('test-microservice'));
    it('should set api base path from context', () => ((queryInstance as any)._apiBasePath).should.equal('/api'));
    it('should set origin from context', () => ((queryInstance as any)._origin).should.equal('https://example.com'));
    it('should set http headers callback from context', () => {
        const headers = (queryInstance as any)._httpHeadersCallback();
        headers.should.deep.equal({ 'X-Custom-Header': 'custom-value' });
    });
});
