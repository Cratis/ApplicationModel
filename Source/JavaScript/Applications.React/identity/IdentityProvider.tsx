// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { IIdentityContext } from './IIdentityContext';
import { useState, useEffect } from 'react';

const defaultIdentityContext: IIdentityContext = {
    id: '',
    name: '',
    claims: {},
    details: {},
    isSet: false,
    refresh() {},
};

export type IdentityProviderResult = {
    id: string;
    name: string;
    claims: { [key: string]: string };
    details: any;
};

export const IdentityProviderContext =
    React.createContext<IIdentityContext>(defaultIdentityContext);

const cookieName = '.cratis-identity';

export interface IdentityProviderProps {
    children?: JSX.Element | JSX.Element[];
}

function getCookie(name: string) {
    const decoded = decodeURIComponent(document.cookie);
    const cookies = decoded.split(';');
    const cookie = cookies.find((_) => _.trim().indexOf(`${name}=`) == 0);
    if (cookie) {
        const keyValue = cookie.split('=');
        return [keyValue[0].trim(), keyValue[1].trim()];
    }
    return [];
}

function clearCookie(name: string) {
    document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT`;
}

export const IdentityProvider = (props: IdentityProviderProps) => {
    const [context, setContext] = useState<IIdentityContext>(defaultIdentityContext);
    const refresh = () => {
        clearCookie(cookieName);
        fetch('/.cratis/me').then(async (response) => {
            try {
                const result = (await response.json()) as IdentityProviderResult;

                if (result) {
                    setContext({
                        id: result.id,
                        name: result.name,
                        claims: result.claims,
                        details: result.details,
                        isSet: true,
                        refresh,
                    });
                }
            } catch (err) {
                console.warn('[IdentityProvider Error /.cratis/me]', err);
            }
        });
    };
    const identityCookie = getCookie(cookieName);
    useEffect(() => {
        if (identityCookie.length == 2) {
            const json = atob(identityCookie[1]);
            const result = JSON.parse(json) as IdentityProviderResult;
            setContext({
                id: result.id,
                name: result.name,
                claims: result.claims,
                details: result.details,
                isSet: true,
                refresh,
            });
        } else {
            refresh();
        }
    }, []);

    return (
        <IdentityProviderContext.Provider value={context}>
            {props.children}
        </IdentityProviderContext.Provider>
    );
};
