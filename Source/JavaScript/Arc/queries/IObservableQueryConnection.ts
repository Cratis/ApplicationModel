// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DataReceived } from './ObservableQueryConnection';

/**
 * Defines a connection for observable queries.
 */
export interface IObservableQueryConnection<TDataType> {
    /**
     * Connect to a specific route.
     * @param {DataReceived<TDataType> dataReceived Callback that will receive the data.
     * @param queryArguments Optional query arguments to pass along.
     */
    connect(dataReceived: DataReceived<TDataType>, queryArguments?: object): void;

    /**
     * Disconnect the connection.
     */
    disconnect();
}
