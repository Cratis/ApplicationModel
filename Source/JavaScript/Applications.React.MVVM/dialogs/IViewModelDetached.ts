// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the view model that is detached from the view. Use this to get the signature of the method that gets called when the view model is detached from the view.
 */
export interface IViewModelDetached {
    /**
     * Method that gets called when the view model is detached from the view.
     */
    detached(): void;
}