// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ICanBeConfigured } from '../ICanBeConfigured';
import { Paging } from './Paging';
import { Sorting } from './Sorting';

/**
 * Defines the commonalities between all query types.
 */
export interface IQuery extends ICanBeConfigured {
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
}
