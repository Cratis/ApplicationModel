// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { injectable } from 'tsyringe';

@injectable()
export class FeatureViewModel {
    constructor() {
        setInterval(() => {
            this.count++;
        }, 1000);
    }

    count: number = 0;
}