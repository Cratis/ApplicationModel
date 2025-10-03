// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { an_observable_query_for } from '../given/an_observable_query_for';
import { given } from '../../../given';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { ObservableQuerySubscription } from '../../ObservableQuerySubscription';
import { Sorting } from '../../Sorting';
import { SortDirection } from '../../SortDirection';

describe('with sorting', given(an_observable_query_for, context => {
    let callback: sinon.SinonStub;
    let subscription: ObservableQuerySubscription<string>;

    beforeEach(() => {
        context.query.setOrigin('https://example.com'); // Set origin to avoid document access
        context.query.sorting = new Sorting('name', SortDirection.ascending);
        callback = sinon.stub();
        
        subscription = context.query.subscribe(callback, { id: 'test-id' });
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
}));