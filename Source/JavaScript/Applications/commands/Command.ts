// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ICommand, PropertyChanged } from './ICommand';
import { CommandResult } from "./CommandResult";
import { CommandValidator } from './CommandValidator';
import { Constructor, JsonSerializer } from '@cratis/fundamentals';
import { Globals } from '../Globals';
import { joinPaths } from '../joinPaths';
import { UrlHelpers } from 'UrlHelpers';

type Callback = {
    callback: WeakRef<PropertyChanged>;
    thisArg: WeakRef<object>;
}

/**
 * Represents an implementation of {@link ICommand} that works with HTTP fetch.
 */
export abstract class Command<TCommandContent = object, TCommandResponse = object> implements ICommand<TCommandContent, TCommandResponse> {
    private _microservice: string;
    private _apiBasePath: string;
    private _origin: string;
    abstract readonly route: string;
    abstract readonly routeTemplate: Handlebars.TemplateDelegate;
    abstract readonly validation: CommandValidator;
    abstract get requestParameters(): string[];
    abstract get properties(): string[];

    private _initialValues: object = {};
    private _hasChanges = false;
    private _callbacks: Callback[] = [];

    /**
     * Initializes a new instance of the {@link Command<,>} class.
     * @param _responseType Type of response.
     * @param _isResponseTypeEnumerable Whether or not the response type is enumerable.
     */
    constructor(readonly _responseType: Constructor = Object, readonly _isResponseTypeEnumerable: boolean) {
        this._microservice = Globals.microservice ?? '';
        this._apiBasePath = '';
        this._origin = '';
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
    async execute(): Promise<CommandResult<TCommandResponse>> {
        let actualRoute = this.route;
        const payload = {};

        this.properties.forEach(property => {
            payload[property] = this[property];
        });

        if (this.requestParameters && this.requestParameters.length > 0) {
            actualRoute = this.routeTemplate(payload);
        }

        const headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        };
        if (this._microservice?.length > 0) {
            headers[Globals.microserviceHttpHeader] = this._microservice;
        }

        actualRoute = joinPaths(this._apiBasePath, actualRoute);
        const url = UrlHelpers.createUrlFrom(this._origin, this._apiBasePath, actualRoute);

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers,
                body: JsonSerializer.serialize(payload)
            });
            this.setInitialValuesFromCurrentValues();

            if( response.status === 404) {
                return CommandResult.failed([`Command not found at route '${actualRoute}'`]) as CommandResult<TCommandResponse>;
            }

            const result = await response.json();
            return new CommandResult(result, this._responseType, this._isResponseTypeEnumerable);
        } catch (ex) {
            return CommandResult.failed([`Error during server call: ${ex}`]) as CommandResult<TCommandResponse>;
        }
    }

    /** @inheritdoc */
    clear(): void {
        this.properties.forEach(property => {
            this[property] = undefined;
        });
        this._initialValues = {};
        this._hasChanges = false;
    }

    /** @inheritdoc */
    setInitialValues(values: TCommandContent) {
        this.properties.forEach(property => {
            if (Object.prototype.hasOwnProperty.call(values, property)) {
                this._initialValues[property] = values[property];
                this[property] = values[property];
            }
        });
        this.updateHasChanges();
    }

    /** @inheritdoc */
    setInitialValuesFromCurrentValues() {
        this.properties.forEach(property => {
            if (this[property]) {
                this._initialValues[property] = this[property];
            }
        });
        this.updateHasChanges();
    }

    /** @inheritdoc */
    revertChanges(): void {
        this.properties.forEach(property => {
            this[property] = this._initialValues[property];
        });
    }

    /** @inheritdoc */
    get hasChanges() {
        return this._hasChanges;
    }

    /** @inheritdoc */
    propertyChanged(property: string) {
        this.updateHasChanges();

        this._callbacks.forEach(callbackContainer => {
            const callback = callbackContainer.callback.deref();
            const thisArg = callbackContainer.thisArg.deref();
            if (callback && thisArg) {
                callback.call(thisArg, property);
            } else {
                this._callbacks = this._callbacks.filter(_ => _.callback !== callbackContainer.callback);
            }
        });
    }

    /** @inheritdoc */
    onPropertyChanged(callback: PropertyChanged, thisArg: object) {
        this._callbacks.push({
            callback: new WeakRef(callback),
            thisArg: new WeakRef(thisArg)
        });
    }

    private updateHasChanges() {
        this._hasChanges = this.properties.some(property => this[property] !== this._initialValues[property]);
    }
}
