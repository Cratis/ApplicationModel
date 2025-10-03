// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import Handlebars from 'handlebars';
import { QueryFor } from '../../QueryFor';
import { Constructor } from '@cratis/fundamentals';

export class TestQueryFor extends QueryFor<string, { id: string }> {
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

export class TestEnumerableQueryFor extends QueryFor<string[], { category: string }> {
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

export class TestQueryForWithoutRequiredParams extends QueryFor<string, object> {
    readonly route = '/api/all';
    readonly routeTemplate = Handlebars.compile('/api/all');
    readonly defaultValue = '';

    get requiredRequestParameters(): string[] {
        return [];
    }

    constructor() {
        super(String as Constructor, false);
    }
}