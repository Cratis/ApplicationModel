// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IIdentityProvider } from './IIdentityProvider';
import { IIdentity } from './IIdentity';
import { IdentityProviderResult } from './IdentityProviderResult';

/**
 * Represents an implementation of {@link IIdentityProvider}.
*/
export class IdentityProvider extends IIdentityProvider {
    static readonly CookieName = '.cratis-identity';

    /**
     * Gets the current identity by optionally specifying the details type.
     * @returns The current identity as {@link IIdentity}.
     */
    static async getCurrent<TDetails = object>(): Promise<IIdentity<TDetails>> {
        const cookie = this.getCookie();
        if (cookie.length == 2) {
            const json = atob(cookie[1]);
            const result = JSON.parse(json) as IdentityProviderResult;
            return {
                id: result.id,
                name: result.name,
                claims: result.claims,
                details: result.details,
                isSet: true,
                refresh: IdentityProvider.refresh
            } as IIdentity<TDetails>;
        } else {
            const identity = await this.refresh<TDetails>();
            return identity;
        }
    }

    /** @inheritdoc */
    async getCurrent<TDetails = object>(): Promise<IIdentity<TDetails>> {
        return IdentityProvider.getCurrent<TDetails>();
    }

    static async refresh<TDetails = object>(): Promise<IIdentity<TDetails>> {
        IdentityProvider.clearCookie();
        const response = await fetch('/.cratis/me');

        const result = await response.json() as IdentityProviderResult;

        return {
            id: result.id,
            name: result.name,
            claims: result.claims,
            details: result.details as TDetails,
            isSet: true,
            refresh: IdentityProvider.refresh
        };
    }

    private static getCookie() {
        const decoded = decodeURIComponent(document.cookie);
        const cookies = decoded.split(';');
        const cookie = cookies.find(_ => _.trim().indexOf(`${IdentityProvider.CookieName}=`) == 0);
        if (cookie) {
            const keyValue = cookie.split('=');
            return [keyValue[0].trim(), keyValue[1].trim()];
        }
        return [];
    }

    private static clearCookie() {
        document.cookie = `${IdentityProvider.CookieName}=;expires=Thu, 01 Jan 1970 00:00:00 GMT`;
    }
}
