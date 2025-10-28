// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { useState, useEffect } from 'react';
import { IIdentity } from '@cratis/applications/identity';
import { IdentityProvider as RootIdentityProvider } from '@cratis/applications/identity';
import { GetHttpHeaders } from '@cratis/applications';

const defaultIdentityContext: IIdentity = {
    id: '',
    name: '',
    details: {},
    isSet: false,
    refresh: () => {
        return new Promise((resolve, reject) => {
            reject('Not implemented');
        });
    }
};

export const IdentityProviderContext = React.createContext<IIdentity>(defaultIdentityContext);

export interface IdentityProviderProps {
    children?: JSX.Element | JSX.Element[],
    httpHeadersCallback?: GetHttpHeaders
}

export const IdentityProvider = (props: IdentityProviderProps) => {
    const [context, setContext] = useState<IIdentity>(defaultIdentityContext);

    useEffect(() => {
        RootIdentityProvider.setHttpHeadersCallback(props.httpHeadersCallback!);
        RootIdentityProvider.getCurrent().then(identity => {
            const refresh = identity.refresh;
            identity.refresh = () => {
                return new Promise<IIdentity>(resolve => {
                    refresh().then(newIdentity => {
                        setContext(newIdentity);
                        resolve(newIdentity);
                    });
                });
            };
            setContext(identity);
        });
    }, []);

    return (
        <IdentityProviderContext.Provider value={context}>
            {props.children}
        </IdentityProviderContext.Provider>
    );
};
