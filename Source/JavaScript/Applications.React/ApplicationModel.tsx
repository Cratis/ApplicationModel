// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Globals } from '@cratis/applications';
import { CommandScope } from './commands';
import { IdentityProvider } from './identity';
import React, { useEffect } from 'react';
import { Bindings } from './Bindings';

export interface ApplicationModelProps {
    children?: JSX.Element | JSX.Element[];
    microservice: string;
}

export interface ApplicationModelConfiguration {
    microservice: string;
}

export const ApplicationModelContext = React.createContext<ApplicationModelConfiguration>({
    microservice: Globals.microservice
});

export const ApplicationModel = (props: ApplicationModelProps) => {
    const configuration: ApplicationModelConfiguration = {
        microservice: props.microservice
    };

    Bindings.initialize(configuration.microservice);

    return (
        <ApplicationModelContext.Provider value={configuration}>
            <IdentityProvider>
                <CommandScope>
                    {props.children}
                </CommandScope>
            </IdentityProvider>
        </ApplicationModelContext.Provider>);
};