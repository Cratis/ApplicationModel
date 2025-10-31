// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IObservableQueryFor, OnNextResult } from './IObservableQueryFor';
import Handlebars from 'handlebars';
import { ObservableQueryConnection } from './ObservableQueryConnection';
import { ObservableQuerySubscription } from './ObservableQuerySubscription';
import { ValidateRequestArguments } from './ValidateRequestArguments';
import { IObservableQueryConnection } from './IObservableQueryConnection';
import { NullObservableQueryConnection } from './NullObservableQueryConnection';
import { Constructor } from '@cratis/fundamentals';
import { JsonSerializer } from '@cratis/fundamentals';
import { QueryResult } from './QueryResult';
import { Sorting } from './Sorting';
import { Paging } from './Paging';
import { SortDirection } from './SortDirection';
import { Globals } from '../Globals';
import { joinPaths } from '../joinPaths';
import { UrlHelpers } from '../UrlHelpers';
import { GetHttpHeaders } from 'GetHttpHeaders';

/* eslint-disable @typescript-eslint/no-explicit-any */

/**
 * Represents an implementation of {@link IQueryFor}.
 * @template TDataType Type of data returned by the query.
 */
export abstract class ObservableQueryFor<TDataType, TParameters = object> implements IObservableQueryFor<TDataType, TParameters> {
    private _microservice: string;
    private _apiBasePath: string;
    private _origin: string;
    private _connection?: IObservableQueryConnection<TDataType>;
    private _httpHeadersCallback: GetHttpHeaders;

    abstract readonly route: string;
    abstract readonly routeTemplate: Handlebars.TemplateDelegate<any>;
    abstract readonly defaultValue: TDataType;
    abstract get requiredRequestParameters(): string[];
    sorting: Sorting;
    paging: Paging;

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

    /**
     * Disposes the query.
     */
    dispose() {
        this._connection?.disconnect();
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
    subscribe(callback: OnNextResult<QueryResult<TDataType>>, args?: TParameters): ObservableQuerySubscription<TDataType> {
        let actualRoute = this.route;
        const connectionQueryArguments: any = {};

        if (this._connection) {
            this._connection.disconnect();
        }

        if (this.paging && this.paging.pageSize > 0) {
            connectionQueryArguments.pageSize = this.paging.pageSize;
            connectionQueryArguments.page = this.paging.page;
        }

        if (this.sorting.hasSorting) {
            connectionQueryArguments.sortBy = this.sorting.field;
            connectionQueryArguments.sortDirection = (this.sorting.direction === SortDirection.descending) ? 'desc' : 'asc';
        }

        
        if (!ValidateRequestArguments(this.constructor.name, this.requiredRequestParameters, args as object)) {
            this._connection = new NullObservableQueryConnection(this.defaultValue);
        } else {
            actualRoute = this.routeTemplate(args);
            actualRoute = joinPaths(this._apiBasePath, actualRoute);
            const url = UrlHelpers.createUrlFrom(this._origin, this._apiBasePath, actualRoute);
            this._connection = new ObservableQueryConnection<TDataType>(url, this._microservice);
        }

        const subscriber = new ObservableQuerySubscription(this._connection);
        this._connection.connect(data => {
            const result: any = data;
            try {
                if (this.enumerable) {
                    if (Array.isArray(result.data)) {
                        result.data = JsonSerializer.deserializeArrayFromInstance(this.modelType, result.data);
                    } else {
                        result.data = [];
                    }
                } else {
                    result.data = JsonSerializer.deserializeFromInstance(this.modelType, result.data);
                }
                callback(result);
            } catch (ex) {
                console.log(ex);
            }
        }, connectionQueryArguments);
        return subscriber;
    }

    /** @inheritdoc */
    async perform(args?: TParameters): Promise<QueryResult<TDataType>> {
        const noSuccess = { ...QueryResult.noSuccess, ...{ data: this.defaultValue } } as QueryResult<TDataType>;

        let actualRoute = this.route;
        if (!ValidateRequestArguments(this.constructor.name, this.requiredRequestParameters, args as object)) {
            return new Promise<QueryResult<TDataType>>((resolve) => {
                resolve(noSuccess);
            });
        }

        actualRoute = this.routeTemplate(args);
        actualRoute = joinPaths(this._apiBasePath, actualRoute);

        if (this.paging.hasPaging) {
            actualRoute = this.addQueryParameter(actualRoute, 'page', this.paging.page);
            actualRoute = this.addQueryParameter(actualRoute, 'pageSize', this.paging.pageSize);
        }

        if (this.sorting.hasSorting) {
            actualRoute = this.addQueryParameter(actualRoute, 'sortBy', this.sorting.field);
            actualRoute = this.addQueryParameter(actualRoute, 'sortDirection', (this.sorting.direction === SortDirection.descending) ? 'desc' : 'asc');
        }

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

        const response = await fetch(url, {
            method: 'GET',
            headers
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
