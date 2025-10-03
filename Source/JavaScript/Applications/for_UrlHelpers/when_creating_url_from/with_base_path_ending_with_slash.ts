// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { UrlHelpers } from '../../UrlHelpers';
import { expect } from 'chai';

describe("with_base_path_ending_with_slash", () => {
    let origin: string;
    let apiBasePath: string;
    let route: string;
    let result: URL;

    beforeEach(() => {
        origin = 'https://example.com';
        apiBasePath = '/api/v1/'; // base path ending with slash
        route = 'users/123'; // relative route
    
        result = UrlHelpers.createUrlFrom(origin, apiBasePath, route);
    });

    it("should_create_correct_url", () => {
        expect(result.href).to.equal('https://example.com/api/v1/users/123');
    });

    it("should_have_correct_origin", () => {
        expect(result.origin).to.equal('https://example.com');
    });

    it("should_have_correct_pathname", () => {
        expect(result.pathname).to.equal('/api/v1/users/123');
    });
});