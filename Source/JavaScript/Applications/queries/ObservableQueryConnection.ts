// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Globals } from '../Globals';
import { IObservableQueryConnection } from './IObservableQueryConnection';
import { QueryResult } from './QueryResult';

export type DataReceived<TDataType> = (data: QueryResult<TDataType>) => void;

/**
 * Represents the connection for an observable query.
 */
export class ObservableQueryConnection<TDataType> implements IObservableQueryConnection<TDataType> {

    private _socket!: WebSocket;
    private _disconnected = false;

    /**
     * Initializes a new instance of the {@link ObservableQueryConnection<TDataType>} class.
     * @param {string} _route The relative route to use - relative to the current origin and protocol.
     */
    constructor(private readonly _route: string, private readonly _microservice: string) {
    }

    /**
     * Disposes the connection.
     */
    dispose() {
        this.disconnect();
    }

    /** @inheritdoc */
    connect(dataReceived: DataReceived<TDataType>, queryArguments?: object) {
        const secure = document?.location?.protocol?.indexOf('https') === 0;
        let url = `${secure ? 'wss' : 'ws'}://${document.location.host}${this._route}`;
        if (this._microservice?.length > 0) {
            url = `${url}?${Globals.microserviceWSQueryArgument}=${this._microservice}`;
        }

        if (queryArguments) {
            if (url.indexOf('?') < 0) {
                url = `${url}?`;
            } else {
                url = `${url}&`;
            }
            const query = Object.keys(queryArguments).map(key => `${key}=${queryArguments[key]}`).join('&');
            url = `${url}${query}`;
        }

        let timeToWait = 500;
        const timeExponent = 500;
        const retries = 100;
        let currentAttempt = 0;
        const maxTime = 10_000;

        const connectSocket = () => {
            const retry = () => {
                currentAttempt++;
                if (currentAttempt > retries) {
                    console.log(`Attempted ${retries} retries for route '${this._route}'. Abandoning.`);
                    return;
                }
                console.log(`Attempting to reconnect for '${this._route}' (#${currentAttempt})`);

                setTimeout(connectSocket, timeToWait);
                timeToWait += (timeExponent * currentAttempt);
                timeToWait = timeToWait > maxTime ? maxTime : timeToWait; 
            };

            this._socket = new WebSocket(url);
            this._socket.onopen = () => {
                if (this._disconnected) return;
                console.log(`Connection for '${this._route}' established`);
                timeToWait = 500;
                currentAttempt = 0;
            };
            this._socket.onclose = () => {
                if (this._disconnected) return;
                console.log(`Unexpected connection closed for route '${this._route}`);
                retry();
            };
            this._socket.onerror = (error) => {
                if (this._disconnected) return;
                console.log(`Error with connection for '${this._route} - ${error}`);
                retry();
            };
            this._socket.onmessage = (ev) => {
                if (this._disconnected) {
                    console.log('Received message after closing connection');
                    return;
                }
                dataReceived(JSON.parse(ev.data));
            };
        };

        if (this._disconnected) return;
        connectSocket();
    }

    /** @inheritdoc */
    disconnect() {
        if (this._disconnected) {
            return;
        }
        console.log(`Disconnecting '${this._route}'`);
        this._disconnected = true;
        this._socket?.close();
        console.log(`Connection for '${this._route}' closed`);
        this._socket = undefined!;
    }
}
