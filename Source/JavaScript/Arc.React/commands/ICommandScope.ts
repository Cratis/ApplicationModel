// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ICommand, CommandResults } from '@cratis/applications/commands';

/**
 * Defines the system for tracking commands in a scope.
 */
export interface ICommandScope {
    /**
     * Gets or sets whether or not there are any changes in the context.
     */
    hasChanges: boolean;

    /**
     * Add a command for tracking in the scope.
     * @param {ICommand} command Command to add.
     */
    addCommand(command: ICommand): void;

    /**
     * Execute all commands with changes.
     * @returns {Promise<CommandResults>} Command results per command that was executed.
     */
    execute(): Promise<CommandResults>;

    /**
     * Revert any changes for commands in scope.
     */
    revertChanges(): void;
}
