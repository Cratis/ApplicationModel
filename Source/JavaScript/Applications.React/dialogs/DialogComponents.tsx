// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { DialogResult } from './DialogResult';
import { ShowDialog } from './ShowDialog';
import { useDialog } from './useDialog';
import { ConfirmationDialogRequest } from './ConfirmationDialogRequest';
import { BusyIndicatorDialogRequest } from './BusyIndicatorDialogRequest';
import { DialogButtons } from './DialogButtons';
import { CloseDialog } from './CloseDialog';

/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface IDialogComponents {
    confirmation?: React.FC<ConfirmationDialogRequest>;
    showConfirmation: ShowDialog<ConfirmationDialogRequest>;
    busyIndicator?: React.FC<BusyIndicatorDialogRequest>;
    showBusyIndicator: ShowDialog<BusyIndicatorDialogRequest>;
    closeBusyIndicator: CloseDialog<object>;
}
/* eslint-enable @typescript-eslint/no-empty-object-type */

export const DialogComponentsContext = React.createContext<IDialogComponents>({
    showConfirmation: () => Promise.resolve([DialogResult.Cancelled, {}]),
    showBusyIndicator: () => Promise.resolve([DialogResult.Cancelled, {}]),
    closeBusyIndicator: () => {}
});

export interface DialogComponentsProps {
    children?: JSX.Element | JSX.Element[];
    confirmation?: React.FC<ConfirmationDialogRequest>;
    busyIndicator?: React.FC<BusyIndicatorDialogRequest>;
}

const DialogComponentsWrapper = (props: DialogComponentsProps) => {
    const [Confirmation, showConfirmation] = useDialog(props.confirmation ?? React.Fragment as React.FC<ConfirmationDialogRequest>);
    const [BusyIndicator, showBusyIndicator, closeBusyIndicatorDialogContext] = useDialog(props.busyIndicator ?? React.Fragment as React.FC<BusyIndicatorDialogRequest>);

    const configuration: IDialogComponents = {
        confirmation: Confirmation,
        showConfirmation,
        busyIndicator: BusyIndicator,
        showBusyIndicator,
        closeBusyIndicator: closeBusyIndicatorDialogContext.closeDialog
    };

    return (
        <DialogComponentsContext.Provider value={configuration}>
            <>
                {props.children}
                {props.confirmation &&
                    <Confirmation title='' message='' buttons={DialogButtons.Ok} />}

                {props.busyIndicator &&
                    <BusyIndicator title='' message='' />}
            </>
        </DialogComponentsContext.Provider>
    );
};

export const DialogComponents = (props: DialogComponentsProps) => {
    return (
        <DialogComponentsWrapper {...props}>
            {props.children}
        </DialogComponentsWrapper>
    );
};
