// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ValidationResult } from '../validation/ValidationResult';
import { IQueryResult } from './IQueryResult';
import { PagingInfo } from './PagingInfo';
import { QueryResult } from './QueryResult';

/**
 * Represents a specialized {@link QueryResult<TDataType>} that holds state for its execution
 */
export class QueryResultWithState<TDataType> implements IQueryResult<TDataType> {

    static empty<TDataType>(defaultValue: TDataType): QueryResultWithState<TDataType> {
        return new QueryResultWithState(
            defaultValue,
            PagingInfo.noPaging,
            true,
            true,
            true,
            [],
            false,
            [],
            '',
            false);
    }

    static initial<TDataType>(defaultValue: TDataType): QueryResultWithState<TDataType> {
        return new QueryResultWithState(
            defaultValue,
            PagingInfo.noPaging,
            true,
            true,
            true,
            [],
            false,
            [],
            '',
            true);
    }

    /**
     * Initializes an instance of {@link QueryResultWithState<TDataType>}.
     * @param {TDataType} data The items returned, if any - can be empty.
     * @param {PagingInfo} paging The paging info.
     * @param {boolean} isSuccess Whether or not the query was successful.
     * @param {boolean} isAuthorized Whether or not the query was authorized.
     * @param {boolean} isValid Whether or not it is valid.
     * @param {ValidationResult[]} validationResults Any validation errors.
     * @param {boolean} hasExceptions Whether or not it has exceptions.
     * @param {string[]} exceptionMessages Any exception messages.
     * @param {string} exceptionStackTrace Exception stack trace, if any.
     * @param {boolean} isPerforming Whether or not the query is being performed. True if its performing, false if it is done.
     */
    constructor(
        readonly data: TDataType,
        readonly paging: PagingInfo,
        readonly isSuccess: boolean,
        readonly isAuthorized: boolean,
        readonly isValid: boolean,
        readonly validationResults: ValidationResult[],
        readonly hasExceptions: boolean,
        readonly exceptionMessages: string[],
        readonly exceptionStackTrace: string,
        readonly isPerforming: boolean) {
    }

    /** @inheritdoc */
    get hasData(): boolean {
        if (Array.isArray(this.data)) {
            return this.data.length > 0;
        }
        return !!this.data;
    }

    /**
     * Create a new {@link QueryResultWithState<TDataType>} from {@link QueryResult<TDataType>}.
     * @param queryResult The original query result.
     * @param isPerforming Whether or not the query is performing.
     * @returns A new {@link QueryResultWithState<TDataType>}
     */
    static fromQueryResult<TDataType>(queryResult: QueryResult<TDataType>, isPerforming: boolean) {
        return new QueryResultWithState<TDataType>(
            queryResult.data,
            queryResult.paging,
            queryResult.isSuccess,
            queryResult.isAuthorized,
            queryResult.isValid,
            queryResult.validationResults,
            queryResult.hasExceptions,
            queryResult.exceptionMessages,
            queryResult.exceptionStackTrace,
            isPerforming);
    }
}
