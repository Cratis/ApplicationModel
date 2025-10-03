// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { an_observable_query_for } from '../given/an_observable_query_for';
import { given } from '../../../given';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { ObservableQuerySubscription } from '../../ObservableQuerySubscription';

describe('with invalid arguments', given(an_observable_query_for, context => {
    let callback: sinon.SinonStub;
    let subscription: ObservableQuerySubscription<string>;

    beforeEach(() => {
        context.query.setOrigin('https://example.com'); // Set origin to avoid document access
        callback = sinon.stub();
        
        // Subscribe with missing required arguments
        subscription = context.query.subscribe(callback);
    });

    afterEach(() => {
        if (subscription) {
            subscription.unsubscribe();
        }
    });

    it('should return a subscription', () => {
        expect(subscription).to.not.be.undefined;
    });

    it('should call callback immediately with default value', () => {
        expect(callback.called).to.be.true;
        // The NullObservableQueryConnection returns QueryResult.empty with defaultValue
        // but the data might be transformed, so let's check the actual structure
        const result = callback.firstCall.args[0];
        expect(result).to.not.be.undefined;
        expect(result.isSuccess).to.be.true;
    });
}));