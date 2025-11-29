// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the paging information for a query.
 */
export class PagingInfo {

    /**
     * No paging paging info.
     */
    static readonly noPaging = new PagingInfo();

    /**
     * Initializes a new instance of the {@link PagingInfo} class.
     */
    constructor() {
        this.page = 0;
        this.size = 0;
        this.totalItems = 0;
        this.totalPages = 0;
    }

    /**
     * Current page.
     */
    page: number;

    /**
     * Page size.
     */
    size: number;

    /**
     * Total number items.
     */
    totalItems: number;

    /**
     * Total number of pages.
     */
    totalPages: number;
}
