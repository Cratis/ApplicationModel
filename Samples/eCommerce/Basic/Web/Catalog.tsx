// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { withViewModel } from '@cratis/applications.react.mvvm';
import { CatalogViewModel } from './CatalogViewModel';
import { AllProducts, ObserveAllProducts } from './API/Products';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { useState } from 'react';

export const Catalog = withViewModel(CatalogViewModel, ({ viewModel }) => {
    const [products, perform, setSorting, setPage, setPageSize] = AllProducts.useWithPaging(10);
    const [descending, setDescending] = useState(false);
    const [currentPage, setCurrentPage] = useState(0);

    return (
        <div>
            <div>Page {currentPage + 1} of {products.paging.totalPages}</div>
            <DataTable value={products.data}>
                <Column field="id" header="SKU" />
                <Column field="name" header="Name" />
            </DataTable>
            Total items: {products.paging.totalItems}
            <br />

            <button onClick={() => {
                setCurrentPage(currentPage - 1);
                setPage(currentPage - 1);
            }}>Previous page</button>

            &nbsp;
            &nbsp;

            <button onClick={() => {
                setCurrentPage(currentPage + 1);
                setPage(currentPage + 1);
            }}>Next page</button>
            <br />

            <button onClick={() => {
                if (products.paging.size == 10) {
                    setPageSize(20);
                } else {
                    setPageSize(10);
                }
            }}>Change pagesize</button>

            &nbsp;

            <button onClick={() => {
                if (descending) {
                    setSorting(AllProducts.sortBy.id.ascending);
                    setPage(0);
                    setDescending(false);
                } else {
                    setSorting(AllProducts.sortBy.id.descending);
                    setPage(0);
                    setDescending(true);
                }
            }}>Change sorting</button>
        </div>
    );
});
