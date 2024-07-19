// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { withViewModel } from '@cratis/applications.react.mvvm';
import { FeatureViewModel } from './FeatureViewModel';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { useIdentity } from '@cratis/applications.react/identity';

export const Feature = withViewModel(FeatureViewModel, ({ viewModel }) => {

    const identity = useIdentity();
    return (
        <div>
            <h2>Hello {`${identity.name}`} your cart id is {`${viewModel.cart.id}`} </h2>

            <DataTable value={viewModel.cart.items}>
                <Column field="SKU" header="SKU" />
                <Column field="price.net" header="Net Price" />
                <Column field="price.gross" header="Gross Price" />
                <Column field="quantity" header="Quantity" />
            </DataTable>
        </div>
    );
});