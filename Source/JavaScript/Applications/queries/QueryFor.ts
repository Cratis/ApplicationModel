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
export abstract class QueryFor<TDataType, TArguments = {}> implements IQueryFor<TDataType, TArguments> {
    abstract readonly route: string;
    abstract readonly routeTemplate: Handlebars.TemplateDelegate;
    abstract get requestArguments(): string[];
    abstract defaultValue: TDataType;
    abortController?: AbortController;
    _sorting: Sorting;
    _paging: Paging | undefined;

    /**
     * Initializes a new instance of the {@link ObservableQueryFor<,>}} class.
     * @param modelType Type of model, if an enumerable, this is the instance type.
     * @param enumerable Whether or not it is an enumerable.
     */
    constructor(readonly modelType: Constructor, readonly enumerable: boolean) {
        this._sorting = Sorting.none;
    }

    /** @inheritdoc */
    get sorting(): Sorting {
        return this._sorting;
    }

    /** @inheritdoc */
    set sorting(value: Sorting) {
        this._sorting = value;
    }

    /** @inheritdoc */
    get paging(): Paging | undefined {
        return this._paging;
    }

    /** @inheritdoc */
    set paging(value: Paging | undefined) {
        this._paging = value;
    }

    /** @inheritdoc */
    async perform(args?: TArguments): Promise<QueryResult<TDataType>> {
        const noSuccess = { ...QueryResult.noSuccess, ...{ data: this.defaultValue } } as QueryResult<TDataType>;

        let actualRoute = this.route;
        if (!ValidateRequestArguments(this.constructor.name, this.requestArguments, args)) {
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
        if (Globals.microservice?.length > 0) {
            headers[Globals.microserviceHttpHeader] = Globals.microservice;
        }

        if (this._paging && this._paging.pageSize > 0) {
            actualRoute = this.addQueryParameters(actualRoute, 'page', this._paging.page);
            actualRoute = this.addQueryParameters(actualRoute, 'pageSize', this._paging.pageSize);
        }

        if (this.sorting.hasSorting) {
            actualRoute = this.addQueryParameters(actualRoute, 'sortBy', this.sorting.field);
            actualRoute = this.addQueryParameters(actualRoute, 'sortDirection', (this.sorting.direction === SortDirection.descending) ? 'desc' : 'asc');
        }

        const response = await fetch(actualRoute, {
            method: 'GET',
            headers,
            signal: this.abortController.signal
        });

        try {
            const result = await response.json();
            return new QueryResult(result, this.modelType, this.enumerable);
        } catch (ex) {
            return noSuccess;
        }
    }

    private addQueryParameters(route: string, key: string, value: any): string {
        route += (route.indexOf('?') > 0) ? '&' : '?';
        route += `${key}=${value}`;
        return route;
    }
}
