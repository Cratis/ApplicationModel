// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { QueryResult } from './QueryResult';
import Handlebars from 'handlebars';
import { IQuery } from './IQuery';

/**
 * Defines the base of a query.
 * @template TDataType Type of model the query is for.
 * @template TArguments Optional type of arguments to use for the query.
 */
export interface IQueryFor<TDataType, TArguments = object> extends IQuery {
    readonly route: string;
    readonly routeTemplate: Handlebars.TemplateDelegate;
    readonly requiredRequestParameters: string[];
    readonly defaultValue: TDataType;

    /**
     * Gets the current arguments for the query.
     */
    get parameters(): TArguments | undefined;

    /**
     * Sets the current arguments for the query.
     */
    set parameters(value: TArguments);

    /**
     * Perform the query, optionally giving arguments to use. If not given, it will use the arguments that has been set.
     * By specifying the arguments, it will use these as the current arguments for the instance and subsequent calls does
     * not need to specify arguments.
     * @param [args] Optional arguments for the query - depends on whether or not the query needs arguments.
     * @returns {QueryResult} for the model
     */
    perform(args?: TArguments): Promise<QueryResult<TDataType>>;
}
