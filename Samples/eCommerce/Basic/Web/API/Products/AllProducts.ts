/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

// eslint-disable-next-line header/header
import { QueryFor, QueryResultWithState } from '@cratis/applications/queries';
import { useQuery, useQueryWithPaging, PerformQuery, SetSorting, SortingForQuery, Paging, SetPage } from '@cratis/applications.react/queries';
import { Product } from './Product';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/products/catalog');

class AllProductsSortBy {
    private _id: SortingForQuery<AllProducts>;
    private _name: SortingForQuery<AllProducts>;
    private _isRegistered: SortingForQuery<AllProducts>;

    constructor(readonly query: AllProducts) {
        this._id = new SortingForQuery(AllProducts, query);
        this._name = new SortingForQuery(AllProducts, query);
        this._isRegistered = new SortingForQuery(AllProducts, query);
    }

    id(): SortingForQuery<AllProducts> {
        return this._id;
    }
    name(): SortingForQuery<AllProducts> {
        return this._name;
    }
    isRegistered(): SortingForQuery<AllProducts> {
        return this._isRegistered;
    }
}

export class AllProducts extends QueryFor<Product[]> {
    readonly route: string = '/api/products/catalog';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: Product[] = [];
    readonly _sortBy: AllProductsSortBy;

    constructor() {
        super(Product, true);
        this._sortBy = new AllProductsSortBy(this);
    }

    get requestArguments(): string[] {
        return [
        ];
    }

    get sortBy(): AllProductsSortBy {
        return this._sortBy;
    }

    static use(): [QueryResultWithState<Product[]>, PerformQuery, SetSorting] {
        return useQuery<Product[], AllProducts>(AllProducts);
    }

    static useWithPaging(page:number, pageSize: number): [QueryResultWithState<Product[]>, PerformQuery, SetSorting, SetPage] {
        return useQueryWithPaging<Product[], AllProducts>(AllProducts, new Paging(page, pageSize));
    }
}
