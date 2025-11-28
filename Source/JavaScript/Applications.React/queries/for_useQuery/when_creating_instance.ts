// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { render } from '@testing-library/react';
import sinon from 'sinon';
import { useQuery } from '../useQuery';
import { FakeQuery } from './FakeQuery';
import { ApplicationModelContext, ApplicationModelConfiguration } from '../../ApplicationModelContext';

describe('when creating instance', () => {
    let capturedQuery: FakeQuery | null = null;
    let fetchStub: sinon.SinonStub;

    beforeEach(() => {
        fetchStub = sinon.stub(global, 'fetch').resolves({
            json: async () => ({ data: [], isSuccess: true, isAuthorized: true, isValid: true, hasExceptions: false, validationResults: [], exceptionMessages: [], exceptionStackTrace: '' })
        } as Response);
    });

    afterEach(() => {
        fetchStub.restore();
    });

    const TestComponent = ({ onQuery }: { onQuery: (query: FakeQuery) => void }) => {
        const [result] = useQuery(FakeQuery);
        return React.createElement('div', null, 'Test');
    };

    const config: ApplicationModelConfiguration = {
        microservice: 'test-microservice',
        apiBasePath: '/api',
        origin: 'https://example.com',
        httpHeadersCallback: () => ({ 'X-Custom-Header': 'custom-value' })
    };

    // We need to capture the query instance via a different approach
    // Since useQuery doesn't expose the query instance directly, we'll spy on the QueryFor constructor
    let queryInstance: FakeQuery | null = null;
    const originalConstructor = FakeQuery.prototype.constructor;
    
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
                useQuery(SpyQuery);
                return React.createElement('div', null, 'Test');
            })
        )
    );

    it('should set microservice from context', () => queryInstance!['_microservice'].should.equal('test-microservice'));
    it('should set api base path from context', () => queryInstance!['_apiBasePath'].should.equal('/api'));
    it('should set origin from context', () => queryInstance!['_origin'].should.equal('https://example.com'));
    it('should set http headers callback from context', () => {
        const headers = queryInstance!['_httpHeadersCallback']();
        headers.should.deep.equal({ 'X-Custom-Header': 'custom-value' });
    });
});
