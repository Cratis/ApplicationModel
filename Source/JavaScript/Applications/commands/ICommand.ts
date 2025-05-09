// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ICanBeConfigured } from '../ICanBeConfigured';
import { CommandResult } from './CommandResult';

/**
 * Callback for when a property changes.
 */
export type PropertyChanged = (property: string) => void;

/**
 * Defines the base of a command.
 */
export interface ICommand<TCommandContent = object, TCommandResponse = object> extends ICanBeConfigured {
    /**
     * Gets the route information for the command.
     */
    readonly route: string;

    /**
     * Execute the {@link ICommand}.
     * @param [args] Optional arguments for the command route - depends on whether or not the command needs arguments.
     * @returns {CommandResult} for the execution.
     */
    execute(): Promise<CommandResult<TCommandResponse>>;

    /**
     * Clear the command properties and reset them to their default values. This will also clear the initial values.
     * This is used when the command is not needed anymore and should be cleared.
     */
    clear(): void;

    /**
     * Set the initial values for the command. This is used for tracking if there are changes to a command or not.
     * @param {*} values Values to set.
     */
    setInitialValues(values: TCommandContent): void;

    /**
     * Set the initial values for the command to be the current value of the properties.
     */
    setInitialValuesFromCurrentValues(): void;

    /**
     * Revert any changes on the command.
     */
    revertChanges(): void;

    /**
     * Gets whether or not there are changes to any properties.
     */
    readonly hasChanges: boolean;

    /**
     * Notify about a property that has had its value changed.
     * @param {string} property Name of property that changes.
     */
    propertyChanged(property: string): void;

    /**
     * Register callback that gets called when a property changes.
     * @param {PropertyChanged} callback Callback to register.
     * @param {*} thisArg The this arg to use when calling.
     */
    onPropertyChanged(callback: PropertyChanged, thisArg: object): void;
}
