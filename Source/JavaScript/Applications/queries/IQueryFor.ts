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
    readonly requiredRequestArguments: string[];
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
    get paging(): Paging;

    /**
     * Sets the paging for the query.
     */ 
    set paging(value: Paging);

    /**
     * Gets the current arguments for the query.
     */    
    get arguments(): TArguments | undefined;

    /**
     * Sets the current arguments for the query.
     */    
    set arguments(value: TArguments);

    /**
     * Set the microservice to be used for the query. This is passed along to the server to identify the microservice.
     * @param microservice Name of microservice
     */
    setMicroservice(microservice: string);

    /**
     * Perform the query, optionally giving arguments to use. If not given, it will use the arguments that has been set.
     * By specifying the arguments, it will use these as the current arguments for the instance and subsequent calls does
     * not need to specify arguments.
     * @param [args] Optional arguments for the query - depends on whether or not the query needs arguments.
     * @returns {QueryResult} for the model
     */
    perform(args?: TArguments): Promise<QueryResult<TDataType>>;
}
