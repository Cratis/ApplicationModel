// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { UrlHelpers } from '../../UrlHelpers';
import { expect } from 'chai';

describe("with_empty_origin", () => {
    let origin: string;
    let apiBasePath: string;
    let route: string;
    let result: URL;
    let originalDocument: Document | undefined;

    beforeEach(() => {
        // Mock document.location.origin
        originalDocument = global.document;
        global.document = {
            location: {
                origin: 'https://mocked-origin.com'
            }
        } as unknown as Document;

        origin = '';
        apiBasePath = '/api/v1';
        route = '/users/123';
    
        result = UrlHelpers.createUrlFrom(origin, apiBasePath, route);
    });

    afterEach(() => {
        if (originalDocument) {
            global.document = originalDocument;
        } else {
            delete (global as { document?: Document }).document;
        }
    });

    it("should_use_document_location_origin", () => {
        expect(result.origin).to.equal('https://mocked-origin.com');
    });

    it("should_create_correct_url_with_document_origin", () => {
        expect(result.href).to.equal('https://mocked-origin.com/users/123');
    });
});