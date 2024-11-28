// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IQueryFor } from './IQueryFor';
import { QueryResult } from "./QueryResult";
import Handlebars from 'handlebars';
import { ValidateRequestArguments } from './ValidateRequestArguments';
import { Constructor } from '@cratis/fundamentals';
import { Paging } from './Paging';
import { Globals } from '../Globals';
import { Sorting } from './Sorting';
import { SortDirection } from './SortDirection';

/**
 * Represents an implementation of {@link IQueryFor}.
 * @template TDataType Type of data returned by the query.
 */
export abstract class QueryFor<TDataType, TArguments = object> implements IQueryFor<TDataType, TArguments> {
    private _microservice: string;
    abstract readonly route: string;
    abstract readonly routeTemplate: Handlebars.TemplateDelegate;
    abstract get requiredRequestArguments(): string[];
    abstract defaultValue: TDataType;
    abortController?: AbortController;
    sorting: Sorting;
    paging: Paging;
    arguments: TArguments | undefined;

    /**
     * Initializes a new instance of the {@link ObservableQueryFor<,>}} class.
     * @param modelType Type of model, if an enumerable, this is the instance type.
     * @param enumerable Whether or not it is an enumerable.
     */
    constructor(readonly modelType: Constructor, readonly enumerable: boolean) {
        this.sorting = Sorting.none;
        this.paging = Paging.noPaging;
        this._microservice = Globals.microservice ?? '';
    }

    /** @inheritdoc */
    setMicroservice(microservice: string) {
        this._microservice = microservice;
    }

    /** @inheritdoc */
    async perform(args?: TArguments): Promise<QueryResult<TDataType>> {
        const noSuccess = { ...QueryResult.noSuccess, ...{ data: this.defaultValue } } as QueryResult<TDataType>;

        args = args || this.arguments;

        let actualRoute = this.route;
        if (!ValidateRequestArguments(this.constructor.name, this.requiredRequestArguments, args as object)) {
            return new Promise<QueryResult<TDataType>>((resolve) => {
                resolve(noSuccess);
            });
        }

        if (this.abortController) {
            this.abortController.abort();
        }

        this.abortController = new AbortController();

        actualRoute = this.routeTemplate(args);

        const headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        };
        if (this._microservice?.length > 0) {
            headers[Globals.microserviceHttpHeader] = this._microservice;
        }

        if (this.paging.hasPaging) {
            actualRoute = this.addQueryParameter(actualRoute, 'page', this.paging.page);
            actualRoute = this.addQueryParameter(actualRoute, 'pageSize', this.paging.pageSize);
        }

        if (this.sorting.hasSorting) {
            actualRoute = this.addQueryParameter(actualRoute, 'sortBy', this.sorting.field);
            actualRoute = this.addQueryParameter(actualRoute, 'sortDirection', (this.sorting.direction === SortDirection.descending) ? 'desc' : 'asc');
        }

        const response = await fetch(actualRoute, {
            method: 'GET',
            headers,
            signal: this.abortController.signal
        });

        try {
            const result = await response.json();
            return new QueryResult(result, this.modelType, this.enumerable);
        } catch {
            return noSuccess;
        }
    }

    private addQueryParameter(route: string, key: string, value: unknown): string {
        route += (route.indexOf('?') > 0) ? '&' : '?';
        route += `${key}=${value}`;
        return route;
    }
}
