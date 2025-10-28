// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Defines the context for identity.
 */
export interface IIdentity<TDetails = object> {

    /**
     * The id of the identity.
     */
    id: string;

    /**
     * The name of the identity.
     */
    name: string;

    /**
     * The application specific details for the identity.
     */
    details: TDetails;

    /**
     * Whether the details are set.
     */
    isSet: boolean;

    /**
     * Refreshes the identity context.
     */
    refresh(): Promise<IIdentity<TDetails>>;
}
