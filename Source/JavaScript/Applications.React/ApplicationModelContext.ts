// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Globals } from '@cratis/applications';
import React from 'react';

export interface ApplicationModelConfiguration {
    microservice: string;
    development?: boolean
    basePath?: string;
    apiBasePath?: string;
}

export const ApplicationModelContext = React.createContext<ApplicationModelConfiguration>({
    microservice: Globals.microservice,
    development: false,
    basePath: '',
    apiBasePath: ''
});
