// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { container } from 'tsyringe';
import { IQueryProvider, QueryProvider } from '@cratis/applications/queries';
import { Constructor } from '@cratis/fundamentals';

export class Bindings {
    static initialize(microservice: string) {
        container.registerSingleton('Microservice', microservice);
        container.register(IQueryProvider as Constructor<IQueryProvider>, { useValue: new QueryProvider(microservice) });
    }
}