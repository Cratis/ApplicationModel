// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Constructor } from '@cratis/fundamentals';
import { useState, useCallback, useContext, useRef, useMemo } from 'react';
import { Command } from '@cratis/applications/commands';
import React from 'react';
import { CommandScopeContext } from './CommandScope';
import { ApplicationModelContext } from '../ApplicationModelContext';

export type SetCommandValues<TCommandContent> = (command: TCommandContent) => void;
export type ClearCommandValues = () => void;

/**
 * Use a command in a component.
 * @param commandType Type of the command to use.
 * @param initialValues Any initial values to set for the command.
 * @returns Tuple with the command, a {@link SetCommandValues<TCommandContent>} delegate to set values on command and {@link ClearCommandValues} delegate clear values.
 */
export function useCommand<TCommand extends Command, TCommandContent>(commandType: Constructor<TCommand>, initialValues?: TCommandContent): [TCommand, SetCommandValues<TCommandContent>, ClearCommandValues] {
    const command = useRef<TCommand | null>(null);
    const [hasChanges, setHasChanges] = useState(false);
    const applicationModel = useContext(ApplicationModelContext);

    const propertyChangedCallback = useCallback(() => {
        if (command.current?.hasChanges !== hasChanges) {
            setHasChanges(command.current?.hasChanges ?? false);
        }
    }, []);

    command.current = useMemo(() => {
        const instance = new commandType();
        instance.setMicroservice(applicationModel.microservice);
        if (initialValues) {
            instance.setInitialValues(initialValues);
        }
        instance.onPropertyChanged(propertyChangedCallback, instance);
        return instance;
    }, []);

    const context = React.useContext(CommandScopeContext);
    context.addCommand?.(command.current!);

    const setCommandValues = (values: TCommandContent) => {
        command!.current!.properties.forEach(property => {
            if (values[property] !== undefined && values[property] != null) {
                command.current![property] = values[property];
            }
        });
    };

    const clearCommandValues = () => {
        command.current!.properties.forEach(property => {
            command.current![property] = undefined;
        });
    };

    return [command.current!, setCommandValues, clearCommandValues];
}
