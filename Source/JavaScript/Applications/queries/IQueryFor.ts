// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Paging } from './Paging';
import { QueryResult } from './QueryResult';
import Handlebars from 'handlebars';
import { Sorting } from './Sorting';

/**
 * Defines the base of a query.
 * @template TDataType Type of model the query is for.
 * @template TArguments Optional type of arguments to use for the query.
 */
export interface IQueryFor<TDataType, TArguments = {}> {
    readonly route: string;
    readonly routeTemplate: Handlebars.TemplateDelegate;
    readonly requestArguments: string[];
    readonly defaultValue: TDataType;

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
    get paging(): Paging | undefined;

    /**
     * Sets the paging for the query.
     */ 
    set paging(value: Paging | undefined);

    /**
     * Perform the query.
     * @param [args] Optional arguments for the query - depends on whether or not the query needs arguments.
     * @returns {QueryResult} for the model
     */
    perform(args?: TArguments): Promise<QueryResult<TDataType>>;
}
