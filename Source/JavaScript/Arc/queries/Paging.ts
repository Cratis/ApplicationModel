// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the paging information for a query.
 */
export class Paging {

    /**
     * No paging.
     */
    static noPaging: Paging = new Paging(0, 0);

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

    /**
     * Gets whether or not there is paging.
     */
    get hasPaging(): boolean {
        return this.page > 0 || this.pageSize > 0;
    }
}