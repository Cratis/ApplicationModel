// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React, { useContext, useEffect, useMemo, useRef } from 'react';
import { Constructor } from '@cratis/fundamentals';
import { DialogResolver } from './DialogRegistration';
import { useDialogMediator } from './DialogMediator';


export interface IDialogContext<TRequest extends object, TResponse> {
    request: TRequest;
    resolver: DialogResolver<TResponse>;
    actualResolver?: DialogResolver<TResponse>;
}

export const DialogContext = React.createContext<IDialogContext<object, object>>(undefined!);

export const useDialogContext = <TRequest extends object, TResponse>(): IDialogContext<TRequest, TResponse> => {
    return useContext(DialogContext) as unknown as IDialogContext<TRequest, TResponse>;
};

interface DialogWrapperProps {
    children?: JSX.Element | JSX.Element[];
    isVisible: boolean;
}

/* eslint-disable @typescript-eslint/no-unused-vars */
const DialogWrapper = <TRequest extends object, TResponse>(props: DialogWrapperProps) => {
    return (
        <div>
            {props.isVisible && props.children}
        </div>
    );
};
/* eslint-enable @typescript-eslint/no-unused-vars */

interface IDialogRequestProps {
    children?: JSX.Element | JSX.Element[];
}

const useConfiguredWrapper = <TRequest extends object, TResponse>(type: Constructor<TRequest>):
    [React.FC<IDialogRequestProps>, IDialogContext<TRequest, TResponse>, DialogResolver<TResponse>] => {
    const mediator = useDialogMediator();
    const [isVisible, setIsVisible] = React.useState(false);

    const dialogContextValue = useRef<IDialogContext<TRequest, TResponse>>(undefined!);

    const requester = (request: TRequest, resolver: DialogResolver<TResponse>) => {
        dialogContextValue.current.request = request;
        dialogContextValue.current.actualResolver = resolver;
        setIsVisible(true);
    };

    const resolver = (response: TResponse) => {
        setIsVisible(false);
        dialogContextValue.current.actualResolver?.(response);
    };

    dialogContextValue.current = useMemo(() => {
        return {
            request: undefined!,
            actualResolver: undefined!,
            resolver
        };
    }, []);

    useEffect(() => {
        mediator.subscribe(type, requester, resolver);
    }, []);

    const ConfiguredWrapper: React.FC<IDialogRequestProps> = useMemo(() => {
        return (props: IDialogRequestProps) => {
            return (
                <DialogWrapper isVisible={isVisible}>
                    <DialogContext.Provider value={dialogContextValue.current as unknown as IDialogContext<object, object>}>
                        {props.children}
                    </DialogContext.Provider>
                </DialogWrapper>);
        };
    }, [isVisible]);

    return [ConfiguredWrapper, dialogContextValue.current, resolver];
};

/**
 * Use a dialog request for showing a dialog.
 * @param request Type of request to use that represents a request that will be made by your view model.
 * @returns A tuple with a component to use for wrapping your dialog and a delegate used when the dialog is resolved with the result expected.
 */
export const useDialogRequest = <TRequest extends object, TResponse>(request: Constructor<TRequest>): [React.FC<IDialogRequestProps>, IDialogContext<TRequest, TResponse>, DialogResolver<TResponse>] => {
    const [DialogWrapper, dialogContext, resolver] = useConfiguredWrapper<TRequest, TResponse>(request);
    return [DialogWrapper, dialogContext, resolver];
};
