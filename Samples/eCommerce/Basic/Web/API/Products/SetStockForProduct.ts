/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

// eslint-disable-next-line header/header
import { Command, CommandValidator, CommandPropertyValidators } from '@cratis/applications/commands';
import { useCommand, SetCommandValues, ClearCommandValues } from '@cratis/applications.react/commands';
import { Validator } from '@cratis/applications/validation';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/set-stock/{sku}');

export interface ISetStockForProduct {
    sku?: string;
    quantity?: number;
}

export class SetStockForProductValidator extends CommandValidator {
    readonly properties: CommandPropertyValidators = {
        sku: new Validator(),
        quantity: new Validator(),
    };
}

export class SetStockForProduct extends Command<ISetStockForProduct> implements ISetStockForProduct {
    readonly route: string = '/set-stock/{sku}';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly validation: CommandValidator = new SetStockForProductValidator();

    private _sku!: string;
    private _quantity!: number;

    constructor() {
        super(Object, false);
    }

    get requestArguments(): string[] {
        return [
            'sku',
        ];
    }

    get properties(): string[] {
        return [
            'sku',
            'quantity',
        ];
    }

    get sku(): string {
        return this._sku;
    }

    set sku(value: string) {
        this._sku = value;
        this.propertyChanged('sku');
    }
    get quantity(): number {
        return this._quantity;
    }

    set quantity(value: number) {
        this._quantity = value;
        this.propertyChanged('quantity');
    }

    static use(initialValues?: ISetStockForProduct): [SetStockForProduct, SetCommandValues<ISetStockForProduct>, ClearCommandValues] {
        return useCommand<SetStockForProduct, ISetStockForProduct>(SetStockForProduct, initialValues);
    }
}
