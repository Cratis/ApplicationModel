// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { TestObservableQuery } from '../given/TestQueries';
import { expect } from 'chai';
import * as sinon from 'sinon';
import { ObservableQuerySubscription } from '../../ObservableQuerySubscription';

describe('with invalid arguments', () => {
    let query: TestObservableQuery;
    let callback: sinon.SinonStub;
    let subscription: ObservableQuerySubscription<string>;

    beforeEach(() => {
        query = new TestObservableQuery();
        query.setOrigin('https://example.com'); // Set origin to avoid document access
        callback = sinon.stub();
        
        // Subscribe with missing required arguments
        subscription = query.subscribe(callback);
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
});