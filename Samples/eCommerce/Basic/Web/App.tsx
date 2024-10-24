// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ApplicationModel } from '@cratis/applications.react';
import { MVVM } from '@cratis/applications.react.mvvm';
import { BrowserRouter, Link, Route, Routes } from "react-router-dom";
import { Feature } from './Feature';
import { DialogComponents } from '@cratis/applications.react.mvvm/dialogs';
import { ConfirmationDialog } from './ConfirmationDialog';
import { BusyIndicatorDialog } from './BusyIndicatorDialog';
import { TestQuery } from './TestQuery';
import { TestParams } from './TestParams';

const Something = () => {
    return (
        <div>
            <Link to='/'>Go back</Link>
        </div>
    );
};

const isDevelopment = process.env.NODE_ENV === 'development';

export const App = () => {
    return (
        <ApplicationModel microservice='e-commerce' development={isDevelopment}>
            <DialogComponents confirmation={ConfirmationDialog} busyIndicator={BusyIndicatorDialog}>
                <MVVM>
                    <BrowserRouter>
                        <Routes>
                            <Route path='/' element={<Feature blah='Horse' />} />
                            <Route path='/something' element={<Something />} />
                            <Route path='/test-query' element={<TestQuery />} />
                            <Route path='/test-params/:id/:num' element={<TestParams />} />
                        </Routes>

                        {/* <Catalog /> */}
                        {/* <ObservingCatalog /> */}
                    </BrowserRouter>
                </MVVM>
            </DialogComponents>
        </ApplicationModel>
    );
};
