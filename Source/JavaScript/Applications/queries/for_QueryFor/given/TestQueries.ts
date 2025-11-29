// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { QueryFor } from '../../QueryFor';
import { Constructor } from '@cratis/fundamentals';
import { ParameterDescriptor } from '../../../reflection/ParameterDescriptor';

export class TestQueryFor extends QueryFor<string, { id: string }> {
    readonly route = '/api/test/{id}';
    readonly defaultValue = '';
    readonly parameterDescriptors: ParameterDescriptor[] = [
        new ParameterDescriptor('id', String as Constructor)
    ];

    get requiredRequestParameters(): string[] {
        return ['id'];
    }

    constructor() {
        super(String as Constructor, false);
    }
}

export class TestEnumerableQueryFor extends QueryFor<string[], { category: string }> {
    readonly route = '/api/items/{category}';
    readonly defaultValue: string[] = [];
    readonly parameterDescriptors: ParameterDescriptor[] = [
        new ParameterDescriptor('category', String as Constructor)
    ];

    get requiredRequestParameters(): string[] {
        return ['category'];
    }

    constructor() {
        super(String as Constructor, true);
    }
}

export class TestQueryForWithoutRequiredParams extends QueryFor<string, object> {
    readonly route = '/api/all';
    readonly defaultValue = '';
    readonly parameterDescriptors: ParameterDescriptor[] = [];

    get requiredRequestParameters(): string[] {
        return [];
    }

    constructor() {
        super(String as Constructor, false);
    }
}