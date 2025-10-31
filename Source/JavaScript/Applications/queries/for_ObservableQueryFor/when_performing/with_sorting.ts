// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { an_observable_query_for } from '../given/an_observable_query_for';
import { given } from '../../../given';
import * as sinon from 'sinon';
import { Sorting } from '../../Sorting';
import { SortDirection } from '../../SortDirection';

describe('with sorting', given(an_observable_query_for, context => {
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
        fetchStub = sinon.stub(global, 'fetch');
        fetchStub.resolves({
            json: sinon.stub().resolves(mockResponse),
            ok: true,
            status: 200
        } as unknown as Response);

        context.query.setOrigin('https://api.example.com');
        context.query.sorting = new Sorting('name', SortDirection.ascending);

        await context.query.perform({ id: 'test-id' });
    });

    afterEach(() => {
        fetchStub.restore();
    });

    it('should include sortBy parameter in URL', () => {
        const call = fetchStub.getCall(0);
        call.args[0].href.should.contain('sortBy=name');
    });

    it('should include sortDirection parameter in URL', () => {
        const call = fetchStub.getCall(0);
        call.args[0].href.should.contain('sortDirection=asc');
    });
}));
