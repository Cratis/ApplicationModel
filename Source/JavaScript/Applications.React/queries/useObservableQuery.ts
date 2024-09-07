// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { QueryResultWithState, IObservableQueryFor, QueryResult, Sorting, Paging, ObservableQuerySubscription } from '@cratis/applications/queries';
import { Constructor } from '@cratis/fundamentals';
import { useState, useEffect, useContext, useRef, useMemo } from 'react';
import { SetSorting } from './SetSorting';
import { SetPage } from './SetPage';
import { SetPageSize } from './SetPageSize';
import { ApplicationModelContext } from '../ApplicationModel';

function useObservableQueryInternal<TDataType, TQuery extends IObservableQueryFor<TDataType>, TArguments = {}>(query: Constructor<TQuery>, sorting?: Sorting, paging?: Paging, args?: TArguments):
    [QueryResultWithState<TDataType>, SetSorting, SetPage, SetPageSize] {
    const [currentPaging, setCurrentPaging] = useState<Paging>(paging ?? Paging.noPaging);
    const [currentSorting, setCurrentSorting] = useState<Sorting>(sorting ?? Sorting.none);
    const applicationModel = useContext(ApplicationModelContext);
    const queryInstance = useRef<TQuery | null>(null);

    queryInstance.current = useMemo(() => {
        const instance = new query() as TQuery;
        instance.paging = currentPaging;
        instance.sorting = currentSorting;
        instance.setMicroservice(applicationModel.microservice);
        return instance;
    }, [currentPaging, currentSorting]);

    const [result, setResult] = useState<QueryResultWithState<TDataType>>(QueryResultWithState.empty(queryInstance.current.defaultValue));
    const argumentsDependency = queryInstance.current.requiredRequestArguments.map(_ => args?.[_]);

    useEffect(() => {
        const subscription = queryInstance.current!.subscribe(response => {
            setResult(QueryResultWithState.fromQueryResult(response, false));
        }, args as any);

        return () => {
            subscription.unsubscribe();
        };
    }, [...argumentsDependency, ...[currentPaging, currentSorting]]);

    return [
        result,
        async (sorting: Sorting) => {
            setCurrentSorting(sorting);
        },
        async (page: number) => {
            setCurrentPaging(new Paging(page, currentPaging.pageSize));
        },
        async (pageSize: number) => {
            setCurrentPaging(new Paging(currentPaging.page, pageSize));
        }];
}

/**
 * React hook for working with {@link IObservableQueryFor} within the state management of React.
 * @template TDataType Type of model the query is for.
 * @template TQuery Type of observable query to use.
 * @template TArguments Optional: Arguments for the query, if any
 * @param query Query type constructor.
 * @param args Optional: Arguments for the query, if any
 * @returns Tuple of {@link QueryResult} and a {@link PerformQuery} delegate.
 */
export function useObservableQuery<TDataType, TQuery extends IObservableQueryFor<TDataType>, TArguments = {}>(query: Constructor<TQuery>, args?: TArguments, sorting?: Sorting):
    [QueryResultWithState<TDataType>, SetSorting] {
    const [result, setSorting, _] = useObservableQueryInternal<TDataType, TQuery, TArguments>(query, sorting, Paging.noPaging, args);
    return [result, setSorting];
}

/**
 * React hook for working with {@link IObservableQueryFor} within the state management of React for queries with paging.
 * @template TDataType Type of model the query is for.
 * @template TQuery Type of observable query to use.
 * @template TArguments Optional: Arguments for the query, if any
 * @param query Query type constructor.
 * @param args Optional: Arguments for the query, if any
 * @param paging Paging information.
 * @returns Tuple of {@link QueryResult} and a {@link PerformQuery} delegate.
 */
export function useObservableQueryWithPaging<TDataType, TQuery extends IObservableQueryFor<TDataType>, TArguments = {}>(query: Constructor<TQuery>, paging: Paging, args?: TArguments, sorting?: Sorting):
    [QueryResultWithState<TDataType>, SetSorting, SetPage, SetPageSize] {
    return useObservableQueryInternal<TDataType, TQuery, TArguments>(query, sorting, paging, args);
}
