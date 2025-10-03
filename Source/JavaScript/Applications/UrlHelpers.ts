// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Url } from 'url';

export class UrlHelpers {
    /**
     * Creates a URL from the given origin, API base path, and route.
     * @param origin The origin of the request. If not provided, it defaults to the current document's origin.
     * @param apiBasePath The base path for the API.
     * @param route The specific route for the request.
     * @returns The constructed URL.
    */
    static createUrlFrom(origin: string, apiBasePath: string, route: string): URL {
        if (!origin || origin.length === 0) {
            origin = document?.location?.origin ?? '';
        }

        return new URL(route, `${origin}${apiBasePath}`);
    }
}