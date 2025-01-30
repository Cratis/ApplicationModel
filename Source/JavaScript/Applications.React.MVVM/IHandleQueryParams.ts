// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents a component that can handle query parameters, typically a viewModel.
 */
export interface IHandleQueryParams<T = object> {

    /**
     * Handle query params.
     * @param queryParams Query Params to handle.
     */
    handleQueryParams(queryParams: T): void;
}
