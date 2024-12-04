// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { WellKnownBindings } from '@cratis/applications.react.mvvm';
import { inject, injectable } from 'tsyringe';
import { Params } from './TestQuery';

@injectable()
export class TestQueryViewModel {
    constructor(@inject(WellKnownBindings.queryParams) params: Params) {
        console.log(params);
    }
}
