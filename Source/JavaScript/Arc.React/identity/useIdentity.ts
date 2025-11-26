// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { IdentityProviderContext } from './IdentityProvider';
import { IIdentity } from '@cratis/applications/identity';

/**
 * Hook to get the identity context.
 * @param defaultDetails Optional default details to use if the context is not set.
 * @returns An identity context.
 */
export function useIdentity<TDetails = object>(defaultDetails?: TDetails | undefined | null): IIdentity<TDetails> {
    const context = React.useContext(IdentityProviderContext) as IIdentity<TDetails>;
    if (context.isSet === false && defaultDetails !== undefined) {
        context.details = defaultDetails!;
    }
    return context;
}
