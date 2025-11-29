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
    private _url: string;

    /**
     * Initializes a new instance of the {@link ObservableQueryConnection<TDataType>} class.
     * @param {Url} url The fully qualified Url.
     */
    constructor(url: URL, private readonly _microservice: string) {
        const secure = url.protocol?.indexOf('https') === 0 || false;

        this._url = `${secure ? 'wss' : 'ws'}://${url.host}${url.pathname}${url.search}`;
        if (this._microservice?.length > 0) {
            const microserviceParam = `${Globals.microserviceWSQueryArgument}=${this._microservice}`;
            if (this._url.indexOf('?') > 0) {
                this._url = `${this._url}&${microserviceParam}`;
            } else {
                this._url = `${this._url}?${microserviceParam}`;
            }
        }
    }

    /**
     * Disposes the connection.
     */
    dispose() {
        this.disconnect();
    }

    /** @inheritdoc */
    connect(dataReceived: DataReceived<TDataType>, queryArguments?: object) {
        let url = this._url;
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
                    console.log(`Attempted ${retries} retries for route '${url}'. Abandoning.`);
                    return;
                }
                console.log(`Attempting to reconnect for '${url}' (#${currentAttempt})`);

                setTimeout(connectSocket, timeToWait);
                timeToWait += (timeExponent * currentAttempt);
                timeToWait = timeToWait > maxTime ? maxTime : timeToWait; 
            };

            this._socket = new WebSocket(url);
            this._socket.onopen = () => {
                if (this._disconnected) return;
                console.log(`Connection for '${url}' established`);
                timeToWait = 500;
                currentAttempt = 0;
            };
            this._socket.onclose = () => {
                if (this._disconnected) return;
                console.log(`Unexpected connection closed for route '${url}'`);
                retry();
            };
            this._socket.onerror = (error) => {
                if (this._disconnected) return;
                console.log(`Error with connection for '${url}' - ${error}`);
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
        console.log(`Disconnecting '${this._url}'`);
        this._disconnected = true;
        this._socket?.close();
        console.log(`Connection for '${this._url}' closed`);
        this._socket = undefined!;
    }
}
