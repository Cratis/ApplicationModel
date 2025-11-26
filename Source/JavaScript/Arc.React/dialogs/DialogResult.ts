// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Defines the possible results from a dialog interaction.
 */
export enum DialogResult {
    /**
     * Indicates that no action was taken or the dialog was closed without a specific result.
     */
    None = 0,

    /**
     * Indicates that the user confirmed the action, typically by clicking "Yes" or "OK".
     */
    Yes = 1,

    /**
     * Indicates that the user declined the action, typically by clicking "No".
     */
    No = 2,

    /**
     * Indicates that the user acknowledged the action, typically by clicking "OK".
     */
    Ok = 3,

    /**
     * Indicates that the dialog was cancelled, typically by clicking "Cancel".
     */
    Cancelled = 4
}
