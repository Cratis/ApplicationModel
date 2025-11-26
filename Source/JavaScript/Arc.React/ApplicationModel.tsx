// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { CommandScope } from './commands';
import { IdentityProvider } from './identity';
import { Bindings } from './Bindings';
import { ApplicationModelConfiguration, ApplicationModelContext } from './ApplicationModelContext';
import { GetHttpHeaders } from '@cratis/applications';

export interface ApplicationModelProps {
    children?: JSX.Element | JSX.Element[];
    microservice?: string;
    development?: boolean;
    origin?: string;
    basePath?: string;
    apiBasePath?: string;
    httpHeadersCallback?: GetHttpHeaders;
}

export const ApplicationModel = (props: ApplicationModelProps) => {
    const configuration: ApplicationModelConfiguration = {
        microservice: props.microservice ?? '',
        development: props.development ?? false,
        origin: props.origin ?? '',
        basePath: props.basePath ?? '',
        apiBasePath: props.apiBasePath ?? '',
        httpHeadersCallback: props.httpHeadersCallback
    };

    Bindings.initialize(
        configuration.microservice,
        configuration.apiBasePath,
        configuration.origin,
        configuration.httpHeadersCallback);

    return (
        <ApplicationModelContext.Provider value={configuration}>
            <IdentityProvider httpHeadersCallback={props.httpHeadersCallback}>
                <CommandScope>
                    {props.children}
                </CommandScope>
            </IdentityProvider>
        </ApplicationModelContext.Provider>);
};