// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestEnumerableQuery } from '../given/TestQueries';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { ObservableQuerySubscription } from '../../ObservableQuerySubscription';

describe('with enumerable query', () => {
    let query: TestEnumerableQuery;
    let callback: sinon.SinonStub;
    let subscription: ObservableQuerySubscription<string[]>;

    beforeEach(() => {
        query = new TestEnumerableQuery();
        query.setOrigin('https://example.com'); // Set origin to avoid document access
        callback = sinon.stub();
        
        subscription = query.subscribe(callback, { category: 'test-category' });
    });

    afterEach(() => {
        if (subscription) {
            subscription.unsubscribe();
        }
    });

    it('should return a subscription', () => {
        expect(subscription).to.not.be.undefined;
    });

    it('should not call callback immediately', () => {
        expect(callback.called).to.be.false;
    });
});