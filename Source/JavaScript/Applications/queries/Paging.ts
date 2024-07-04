// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the paging information for a query.
 */
export class Paging {

    /**
     * Initializes a new instance of {@link Paging}.
     */
    constructor(page?: number, pageSize?: number) {
        this.page = page ?? 0;
        this.pageSize = pageSize ?? 0;
    }

    /**
     * Page  of paging
     */
    page: number;

    /**
     * Page size of paging
     */
    pageSize: number;
}