/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

// eslint-disable-next-line header/header
import { QueryFor, QueryResultWithState, SortingActions, SortingActionsForQuery, Paging } from '@cratis/applications/queries';
import { useQuery, useQueryWithPaging, PerformQuery, SetSorting, SetPage } from '@cratis/applications.react/queries';
import { Product } from './Product';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/products/catalog');

class AllProductsSortBy {
    private _id: SortingActionsForQuery<Product[]>;
    private _name: SortingActionsForQuery<Product[]>;
    private _isRegistered: SortingActionsForQuery<Product[]>;

    constructor(readonly query: AllProducts) {
        this._id = new SortingActionsForQuery<Product[]>('id', query);
        this._name = new SortingActionsForQuery<Product[]>('name', query);
        this._isRegistered = new SortingActionsForQuery<Product[]>('isRegistered', query);
    }

    get id(): SortingActionsForQuery<Product[]> {
        return this._id;
    }
    get name(): SortingActionsForQuery<Product[]> {
        return this._name;
    }
    get isRegistered(): SortingActionsForQuery<Product[]> {
        return this._isRegistered;
    }
}

class AllProductsSortByWithoutQuery {
    private _id: SortingActions  = new SortingActions('id');
    private _name: SortingActions  = new SortingActions('name');
    private _isRegistered: SortingActions  = new SortingActions('isRegistered');

    get id(): SortingActions {
        return this._id;
    }
    get name(): SortingActions {
        return this._name;
    }
    get isRegistered(): SortingActions {
        return this._isRegistered;
    }
}


export class AllProducts extends QueryFor<Product[]> {
    readonly route: string = '/api/products/catalog';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: Product[] = [];
    private readonly _sortBy: AllProductsSortBy;
    private static readonly _sortBy: AllProductsSortByWithoutQuery = new AllProductsSortByWithoutQuery();

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

    static get sortBy(): AllProductsSortByWithoutQuery {
        return this._sortBy;
    }

    static use(): [QueryResultWithState<Product[]>, PerformQuery, SetSorting] {
        return useQuery<Product[], AllProducts>(AllProducts);
    }

    static useWithPaging(pageSize: number): [QueryResultWithState<Product[]>, number, PerformQuery, SetSorting, SetPage] {
        return useQueryWithPaging<Product[], AllProducts>(AllProducts, new Paging(0, pageSize));
    }
}
