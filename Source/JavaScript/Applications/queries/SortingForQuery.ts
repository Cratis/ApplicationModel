// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IQueryFor } from './IQueryFor';
import { SortDirection } from './SortDirection';
import { Sorting } from './Sorting';

/**
 * Represents sorting for a query.
 */

export class SortingForQuery<TDataType, TArguments = {}> {

    /**
     *
     * @param {string} field The field that the sorting represents.
     * @param {IQueryFor<TDataType, TArguments>} query The query that holds the field.
     */
    constructor(readonly field: string, readonly query: IQueryFor<TDataType, TArguments>) {
    }

    /**
      * Instructs query to sort ascending for the field.
      */
    get ascending(): SortingForQuery<TDataType, TArguments> {
        this.query.sorting = new Sorting(this.field, SortDirection.ascending);
        return this;
    }

    /**
      * Instructs query to sort ascending for the field.
      */
    get descending(): SortingForQuery<TDataType, TArguments> {
        this.query.sorting = new Sorting(this.field, SortDirection.descending);
        return this;
    }
}
