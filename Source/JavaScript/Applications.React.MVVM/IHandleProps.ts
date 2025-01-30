// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents a component that can handle props, typically a viewModel.
 */
export interface IHandleProps<T = object> {

    /**
     * Handle props.
     * @param props Props to handle.
     */
    handleProps(props: T): void;
}
