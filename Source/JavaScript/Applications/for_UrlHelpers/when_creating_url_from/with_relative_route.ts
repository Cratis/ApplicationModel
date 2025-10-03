// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { UrlHelpers } from '../../UrlHelpers';


describe("with_relative_route", () => {
    let origin: string;
    let apiBasePath: string;
    let route: string;
    let result: URL;

    beforeEach(() => {
        origin = 'https://example.com';
        apiBasePath = '/api/v1';
        route = 'users/123'; // relative route without leading slash
    
        result = UrlHelpers.createUrlFrom(origin, apiBasePath, route);
    });

    it("should_create_correct_url_with_relative_route", () => {
        result.href.should.equal('https://example.com/api/users/123');
    });

    it("should_have_correct_origin", () => {
        result.origin.should.equal('https://example.com');
    });

    it("should_have_correct_pathname", () => {
        result.pathname.should.equal('/api/users/123');
    });
});