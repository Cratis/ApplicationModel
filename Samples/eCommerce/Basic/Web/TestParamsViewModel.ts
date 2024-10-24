// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


import { WellKnownBindings } from '@cratis/applications.react.mvvm';
import { inject, injectable } from 'tsyringe';
import { Params } from './TestParams';

@injectable()
export class TestParamsViewModel {
    constructor(@inject(WellKnownBindings.params) params: Params) {
        console.log(params);
    }
}
