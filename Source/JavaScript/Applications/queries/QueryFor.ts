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
import { joinPaths } from '../joinPaths';
import { UrlHelpers } from '../UrlHelpers';
import { GetHttpHeaders } from '../GetHttpHeaders';

/**
 * Represents an implementation of {@link IQueryFor}.
 * @template TDataType Type of data returned by the query.
 */
export abstract class QueryFor<TDataType, TParameters = object> implements IQueryFor<TDataType, TParameters> {
    private _microservice: string;
    private _apiBasePath: string;
    private _origin: string;
    private _httpHeadersCallback: GetHttpHeaders;
    abstract readonly route: string;
    abstract readonly routeTemplate: Handlebars.TemplateDelegate;
    abstract get requiredRequestParameters(): string[];
    abstract defaultValue: TDataType;
    abortController?: AbortController;
    sorting: Sorting;
    paging: Paging;
    parameters: TParameters | undefined;

    /**
     * Initializes a new instance of the {@link ObservableQueryFor<,>}} class.
     * @param modelType Type of model, if an enumerable, this is the instance type.
     * @param enumerable Whether or not it is an enumerable.
     */
    constructor(readonly modelType: Constructor, readonly enumerable: boolean) {
        this.sorting = Sorting.none;
        this.paging = Paging.noPaging;
        this._microservice = Globals.microservice ?? '';
        this._apiBasePath = '';
        this._origin = '';
        this._httpHeadersCallback = () => ({});
    }

    /** @inheritdoc */
    setMicroservice(microservice: string) {
        this._microservice = microservice;
    }

    /** @inheritdoc */
    setApiBasePath(apiBasePath: string): void {
        this._apiBasePath = apiBasePath;
    }

    /** @inheritdoc */
    setOrigin(origin: string): void {
        this._origin = origin;
    }

    /** @inheritdoc */
    setHttpHeadersCallback(callback: GetHttpHeaders): void {
        this._httpHeadersCallback = callback;
    }

    /** @inheritdoc */
    async perform(args?: TParameters): Promise<QueryResult<TDataType>> {
        const noSuccess = { ...QueryResult.noSuccess, ...{ data: this.defaultValue } } as QueryResult<TDataType>;

        args = args || this.parameters;

        let actualRoute = this.route;
        if (!ValidateRequestArguments(this.constructor.name, this.requiredRequestParameters, args as object)) {
            return new Promise<QueryResult<TDataType>>((resolve) => {
                resolve(noSuccess);
            });
        }

        if (this.abortController) {
            this.abortController.abort();
        }

        this.abortController = new AbortController();

        actualRoute = this.routeTemplate(args);
        actualRoute = joinPaths(this._apiBasePath, actualRoute);
        const url = UrlHelpers.createUrlFrom(this._origin, this._apiBasePath, actualRoute);

        const headers = {
            ... this._httpHeadersCallback?.(), ...
            {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
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

        const response = await fetch(url, {
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
