// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React, { useMemo, useRef } from 'react';
import { ConfirmationDialogRequest } from './ConfirmationDialogRequest';
import { BusyIndicatorDialogRequest } from './BusyIndicatorDialogRequest';
import { DialogResult } from '@cratis/applications.react/dialogs';
import { useDialogRequest } from './useDialogRequest';
import { DialogMediator } from './DialogMediator';
import { DialogMediatorHandler } from './DialogMediatorHandler';
import { IDialogMediatorHandler } from './IDialogMediatorHandler';

export interface IDialogComponentsContext {
}

export const DialogComponentsContext = React.createContext<IDialogComponentsContext>({});

export interface DialogComponentsProps {
    children?: JSX.Element | JSX.Element[];
    confirmation?: React.FC | React.FC<any>;
    busyIndicator?: React.FC | React.FC<any>;
}

const DialogComponentsWrapper = (props: DialogComponentsProps) => {
    const [ConfirmationDialog] = useDialogRequest<ConfirmationDialogRequest, DialogResult>(ConfirmationDialogRequest);
    const [BusyIndicatorDialog] = useDialogRequest<BusyIndicatorDialogRequest, DialogResult>(BusyIndicatorDialogRequest);

    return (
        <DialogComponentsContext.Provider value={{}}>
            <>
                {props.children}
                {props.confirmation &&
                    <ConfirmationDialog>
                        <props.confirmation />
                    </ConfirmationDialog>}

                {props.busyIndicator &&
                    <BusyIndicatorDialog>
                        <props.busyIndicator />
                    </BusyIndicatorDialog>}
            </>
        </DialogComponentsContext.Provider>
    );
};

export const DialogComponents = (props: DialogComponentsProps) => {

    const mediatorHandler = useRef<IDialogMediatorHandler | null>(null);
    mediatorHandler.current = useMemo(() => {
        return new DialogMediatorHandler();
    }, []);

    return (
        <DialogMediator handler={mediatorHandler.current!}>
            <DialogComponentsWrapper {...props}>
                {props.children}
            </DialogComponentsWrapper>
        </DialogMediator>
    );
};
