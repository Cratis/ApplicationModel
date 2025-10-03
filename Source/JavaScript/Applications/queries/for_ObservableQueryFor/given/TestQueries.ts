// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import Handlebars from 'handlebars';
import { ObservableQueryFor } from '../../ObservableQueryFor';
import { Constructor } from '@cratis/fundamentals';

export class TestObservableQuery extends ObservableQueryFor<string, { id: string }> {
    readonly route = '/api/test/{id}';
    readonly routeTemplate = Handlebars.compile('/api/test/{{{id}}}');
    readonly defaultValue = '';

    get requiredRequestParameters(): string[] {
        return ['id'];
    }

    constructor() {
        super(String as Constructor, false);
    }
}

export class TestEnumerableQuery extends ObservableQueryFor<string[], { category: string }> {
    readonly route = '/api/items/{category}';
    readonly routeTemplate = Handlebars.compile('/api/items/{{{category}}}');
    readonly defaultValue: string[] = [];

    get requiredRequestParameters(): string[] {
        return ['category'];
    }

    constructor() {
        super(String as Constructor, true);
    }
}