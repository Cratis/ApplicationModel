// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Paging } from './Paging';
import { Sorting } from './Sorting';

/**
 * Defines the commonalities between all query types.
 */
export interface IQuery {
    /**
     * Gets the sorting for the query.
     */
    get sorting(): Sorting;

    /**
     * Sets the sorting for the query.
     */
    set sorting(value: Sorting);

    /**
     * Gets the paging for the query.
     */
    get paging(): Paging;

    /**
     * Sets the paging for the query.
     */
    set paging(value: Paging);

    /**
     * Set the microservice to be used for the query. This is passed along to the server to identify the microservice.
     * @param microservice Name of microservice
     */
    setMicroservice(microservice: string);

    /**
     * Set the base path for the API to use for the query. This is used to prepend to the path of the query.
     * @param apiBasePath Base path for the API
     */
    setApiBasePath(apiBasePath: string): void;
}
