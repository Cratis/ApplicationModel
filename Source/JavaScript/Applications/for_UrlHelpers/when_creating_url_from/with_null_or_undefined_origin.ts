// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { UrlHelpers } from '../../UrlHelpers';


describe("with_null_or_undefined_origin", () => {
    let apiBasePath: string;
    let route: string;
    let result: URL;
    let originalDocument: Document | undefined;

    beforeEach(() => {
        // Mock document.location.origin
        originalDocument = global.document;
        global.document = {
            location: {
                origin: 'https://fallback-origin.com'
            }
        } as unknown as Document;

        apiBasePath = '/api/v1';
        route = '/users/123';
    });

    afterEach(() => {
        if (originalDocument) {
            global.document = originalDocument;
        } else {
            delete (global as { document?: Document }).document;
        }
    });

    it("should_use_document_location_origin_when_null", () => {
        result = UrlHelpers.createUrlFrom(null as unknown as string, apiBasePath, route);
        result.origin.should.equal('https://fallback-origin.com');
    });

    it("should_use_document_location_origin_when_undefined", () => {
        result = UrlHelpers.createUrlFrom(undefined as unknown as string, apiBasePath, route);
        result.origin.should.equal('https://fallback-origin.com');
    });

    it("should_create_correct_url_with_document_origin_when_null", () => {
        result = UrlHelpers.createUrlFrom(null as unknown as string, apiBasePath, route);
        result.href.should.equal('https://fallback-origin.com/users/123');
    });

    it("should_create_correct_url_with_document_origin_when_undefined", () => {
        result = UrlHelpers.createUrlFrom(undefined as unknown as string, apiBasePath, route);
        result.href.should.equal('https://fallback-origin.com/users/123');
    });
});