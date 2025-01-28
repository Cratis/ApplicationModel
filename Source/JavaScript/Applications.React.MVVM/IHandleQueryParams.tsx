/**
 * Represents a component that can handle query parameters, typically a viewModel.
 */


export interface IHandleQueryParams<T = object> {

    /**
     * Handle params.
     * @param params Params to handle.
     */
    handleQueryParams(params: T): void;
}
