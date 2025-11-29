// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export interface IGlobals {
    microservice: string;
    microserviceHttpHeader: string;
    microserviceWSQueryArgument: string;
}

export const Globals: IGlobals = {
    microservice: '',
    microserviceHttpHeader: 'x-cratis-microservice',
    microserviceWSQueryArgument: 'x-cratis-microservice'
};