// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents a component that can handle parameters, typically a viewModel.
 */
export interface IHandleParams<T = object> {

    /**
     * Handle params.
     * @param params Params to handle.
     */
    handleParams(params: T): void;
}
