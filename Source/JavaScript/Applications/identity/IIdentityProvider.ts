// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IIdentity } from './IIdentity';

/**
 * Defines the identity provider.
 */
export abstract class IIdentityProvider {

    /**
     * Gets the current identity by optionally specifying the details type.
     * @returns The current identity as {@link IIdentity}.
     */
    abstract getCurrent<TDetails = {}>(): Promise<IIdentity<TDetails>>;
}
