// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_query_for } from '../given/a_query_for';
import { given } from '../../../given';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { QueryResult } from '../../QueryResult';

describe('with json parse error', given(a_query_for, context => {
    let result: QueryResult<string>;
    let fetchStub: sinon.SinonStub;

    beforeEach(async () => {
        // Setup fetch mock with json parse error
        fetchStub = sinon.stub(global, 'fetch');
        fetchStub.resolves({
            json: sinon.stub().rejects(new Error('Invalid JSON')),
            ok: true,
            status: 200
        } as unknown as Response);

        context.query.setOrigin('https://api.example.com');

        // Call perform with valid arguments
        result = await context.query.perform({ id: 'test-id' });
    });

    afterEach(() => {
        fetchStub.restore();
    });

    it('should return no success result', () => {
        expect(result.isSuccess).to.be.false;
    });

    it('should return default value', () => {
        expect(result.data).to.equal('');
    });

    it('should return result without data', () => {
        expect(result.data).to.equal('');
        expect(result.isSuccess).to.be.false;
    });
}));