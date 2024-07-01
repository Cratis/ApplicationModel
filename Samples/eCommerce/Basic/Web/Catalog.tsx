// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { withViewModel } from '@cratis/applications.react.mvvm';
import { CatalogViewModel } from './CatalogViewModel';
import { AllProducts } from './API/Products';

export const Catalog = withViewModel(CatalogViewModel, ({ viewModel }) => {
    const [products] = AllProducts.useWithPaging(0, 10);

    return (
        <></>
    );
});
