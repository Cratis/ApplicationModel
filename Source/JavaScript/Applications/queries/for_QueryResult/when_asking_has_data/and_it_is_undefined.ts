// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { QueryResult } from '../../QueryResult';

/* eslint-disable @typescript-eslint/no-explicit-any */

describe("when asking has data and it is undefined", () => {
    const queryResult = new QueryResult<any>({
        validationResults: [],
        data:undefined
    }, Object, true);

    it('should considered to not having data', () => queryResult.hasData.should.be.false);
});
