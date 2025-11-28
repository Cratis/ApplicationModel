// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { render } from '@testing-library/react';
import sinon from 'sinon';
import { useQuery } from '../useQuery';
import { FakeQuery } from './FakeQuery';
import { ApplicationModelContext, ApplicationModelConfiguration } from '../../ApplicationModelContext';

describe('when creating instance without optional context values', () => {
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
        microservice: 'test-microservice'
    };

    let queryInstance: FakeQuery | null = null;
    
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

    it('should set api base path to empty string', () => queryInstance!['_apiBasePath'].should.equal(''));
    it('should set origin to empty string', () => queryInstance!['_origin'].should.equal(''));
    it('should set http headers callback to return empty object', () => {
        const headers = queryInstance!['_httpHeadersCallback']();
        headers.should.deep.equal({});
    });
});
