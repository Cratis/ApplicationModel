// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { render } from '@testing-library/react';
import sinon from 'sinon';
import { useQuery } from '../useQuery';
import { FakeQuery } from './FakeQuery';
import { ApplicationModelContext, ApplicationModelConfiguration } from '../../ApplicationModelContext';
import { Sorting } from '@cratis/applications/queries';

describe('when creating instance with sorting', () => {
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
        microservice: 'test-microservice'
    };

    const sorting = new Sorting('name', 1);
    
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
                useQuery(SpyQuery, undefined, sorting);
                return React.createElement('div', null, 'Test');
            })
        )
    );

    it('should set sorting on the query', () => queryInstance!.sorting.should.equal(sorting));
});
