// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { withViewModel } from '@cratis/applications.react.mvvm';
import { FeatureViewModel } from './FeatureViewModel';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { useIdentity } from '@cratis/applications.react/identity';
import { useDialogRequest } from '@cratis/applications.react.mvvm/dialogs';
import { CustomDialog, CustomDialogRequest } from './CustomDialog';
import { Link } from 'react-router-dom';
import { ObserveCartForCurrentUser } from './API/Carts';


export interface FeatureProps {
    blah: string;
}

export const Feature = withViewModel<FeatureViewModel, FeatureProps>(FeatureViewModel, ({ viewModel, props }) => {
    const [CustomDialogWrapper, context, resolver] = useDialogRequest<CustomDialogRequest, string>(CustomDialogRequest);
    const [result] = ObserveCartForCurrentUser.use();
    const identity = useIdentity();
    return (
        <div>
            <h2>Hello {`${identity.name}`} your cart id is {`${viewModel.cart.id}`} - counter: {viewModel.counter}</h2>
            <Link to='/something'>Go to something</Link>

            <DataTable value={result.data.items}>
                <Column field="SKU" header="SKU" />
                <Column field="price.net" header="Net Price" />
                <Column field="price.gross" header="Gross Price" />
                <Column field="quantity" header="Quantity" />
            </DataTable>

            <button onClick={() => viewModel.doStuff()}>Open dialog</button>
            <br />
            <button onClick={() => viewModel.doOtherStuff()}>Open standard dialog</button>
            <br />
            <button onClick={() => viewModel.doHeavyStuff()}>Show busy indicator</button>


            <CustomDialogWrapper>
                <CustomDialog />
            </CustomDialogWrapper>
        </div>
    );
});