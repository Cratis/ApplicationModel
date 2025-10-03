// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestObservableQuery } from './given/TestQueries';
import { expect } from 'chai';
import * as sinon from 'sinon';

describe('when disposing', () => {
    let query: TestObservableQuery;
    let callback: sinon.SinonStub;

    beforeEach(() => {
        query = new TestObservableQuery();
        query.setOrigin('https://example.com'); // Set origin to avoid document access
        callback = sinon.stub();
        
        // Create a subscription first
        query.subscribe(callback, { id: 'test-id' });
    });

    it('should not throw when disposing', () => {
        expect(() => query.dispose()).to.not.throw();
    });

    it('should be safe to dispose multiple times', () => {
        query.dispose();
        expect(() => query.dispose()).to.not.throw();
    });
});