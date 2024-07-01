// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IQueryFor, QueryResultWithState, QueryResult, Paging, Sorting } from '@cratis/applications/queries';
import { Constructor } from '@cratis/fundamentals';
import { useState, useEffect } from 'react';

/**
 * Delegate type for performing a {@link IQueryFor} in the context of the {@link useQuery} hook.
 */
export type PerformQuery<TArguments = {}> = (args?: TArguments) => Promise<void>;

/**
 * Delegate type for setting the sorting in the context of the {@link useQuery} and {@link useQueryWithPaging} hooks.
 */
export type SetSorting = (sorting: Sorting) => Promise<void>;

/**
 * Delegate type for setting the page in the context of the {@link useQueryWithPaging} hook.
 */
export type SetPage = (page: number) => Promise<void>;

type QueryPerformer<TDataType> = () => Promise<QueryResult<TDataType>>;

function useQueryInternal<TDataType, TQuery extends IQueryFor<TDataType>, TArguments = {}>(query: Constructor<TQuery>, performer: QueryPerformer<TDataType>, sorting?: Sorting, paging?: Paging, args?: TArguments):
    [QueryResultWithState<TDataType>, PerformQuery<TArguments>, SetSorting, SetPage] {
    const queryInstance = new query() as TQuery;
    const [currentSorting, setCurrentSorting] = useState<Sorting>(sorting ?? Sorting.none);
    const [currentPaging, setCurrentPaging] = useState<Paging | undefined>(paging);
    const [result, setResult] = useState<QueryResultWithState<TDataType>>(QueryResultWithState.initial(queryInstance.defaultValue));
    const queryExecutor = (async (args?: TArguments) => {
        queryInstance.sorting = currentSorting;
        queryInstance.paging = currentPaging!;
        const queryResult = await performer();
        setResult(QueryResultWithState.fromQueryResult(queryResult, false));
    });

    useEffect(() => {
        queryExecutor(args);
    }, []);

    return [
        result,
        async (args?: TArguments) => {
            setResult(QueryResultWithState.fromQueryResult(result, true));
            await queryExecutor(args);
        },
        async (sorting: Sorting) => {
            setResult(QueryResultWithState.fromQueryResult(result, true));
            setCurrentSorting(sorting);
            await queryExecutor(args);
        },
        async (page: number) => {
            setResult(QueryResultWithState.fromQueryResult(result, true));
            setCurrentPaging({ page, pageSize: currentPaging!.pageSize });
            await queryExecutor(args);
        }];
}

/**
 * React hook for working with {@link IQueryFor} within the state management of React.
 * @template TDataType Type of model the query is for.
 * @template TQuery Type of query to use.
 * @template TArguments Optional: Arguments for the query, if any
 * @param query Query type constructor.
 * @returns Tuple of {@link QueryResult} and a {@link PerformQuery} delegate.
 */
export function useQuery<TDataType, TQuery extends IQueryFor<TDataType>, TArguments = {}>(query: Constructor<TQuery>, args?: TArguments, sorting?: Sorting):
    [QueryResultWithState<TDataType>, PerformQuery<TArguments>, SetSorting] {
    const queryInstance = new query() as TQuery;
    const [result, perform, setSorting] = useQueryInternal(query, async () => await queryInstance.perform(args!), sorting, undefined, args);
    return [result, perform, setSorting];
}

/**
 * React hook for working with {@link IQueryFor} within the state management of React for queries with paging.
 * @template TDataType Type of model the query is for.
 * @template TQuery Type of query to use.
 * @template TArguments Optional: Arguments for the query, if any
 * @param query Query type constructor.
 * @returns Tuple of {@link QueryResult} and a {@link PerformQuery} delegate.
 */
export function useQueryWithPaging<TDataType, TQuery extends IQueryFor<TDataType>, TArguments = {}>(query: Constructor<TQuery>, paging: Paging, args?: TArguments, sorting?: Sorting):
    [QueryResultWithState<TDataType>, PerformQuery<TArguments>, SetSorting, SetPage] {
    const queryInstance = new query() as TQuery;
    return useQueryInternal(query, async () => await queryInstance.perform(args!), sorting, undefined, args);
}
