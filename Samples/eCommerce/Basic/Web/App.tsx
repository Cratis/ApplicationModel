// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ApplicationModel } from '@cratis/applications.react';
import { MVVM } from '@cratis/applications.react.mvvm';
import { BrowserRouter } from "react-router-dom";
import { Feature } from './Feature';
import { DialogComponents } from '@cratis/applications.react.mvvm/dialogs';
import { ConfirmationDialog } from './ConfirmationDialog';
import { BusyIndicatorDialog } from './BusyIndicatorDialog';

export const App = () => {
    return (
        <ApplicationModel microservice='e-commerce'>
            <DialogComponents confirmation={ConfirmationDialog} busyIndicator={BusyIndicatorDialog}>
                <MVVM>
                    <BrowserRouter>
                        <Feature blah='Horse' />
                        {/* <Catalog /> */}
                        {/* <ObservingCatalog /> */}
                    </BrowserRouter>
                </MVVM>
            </DialogComponents>
        </ApplicationModel>
    );
};
