// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { container, DependencyContainer } from "tsyringe";
import { Constructor, Guid } from '@cratis/fundamentals';
import { FunctionComponent, ReactElement, useEffect, useMemo, useRef, useState } from 'react';
import { Observer } from 'mobx-react';
import { has, makeAutoObservable } from 'mobx';
import { useParams } from 'react-router-dom';
import {
    DialogMediator,
    DialogMediatorHandler,
    Dialogs,
    IDialogMediatorHandler,
    IDialogs,
    useDialogMediator
} from './dialogs';
import { IViewModelDetached } from './IViewModelDetached';

function disposeViewModel(viewModel: any) {
    const vmWithDetach = (viewModel as IViewModelDetached);
    if (typeof (vmWithDetach.detached) == 'function') {
        vmWithDetach.detached();
    }

    if (viewModel.__childContainer) {
        const container = viewModel.__childContainer as DependencyContainer;
        container.dispose();
    }
}

/**
 * Represents the view context that is passed to the view.
 */
export interface IViewContext<T, TProps = any> {
    viewModel: T,
    props: TProps,
}

/**
 * Use a view model with a component.
 * @param {Constructor} viewModelType View model type to use.
 * @param {FunctionComponent} targetComponent The target component to render.
 * @returns 
 */
export function withViewModel<TViewModel extends {}, TProps extends {} = {}>(viewModelType: Constructor<TViewModel>, targetComponent: FunctionComponent<IViewContext<TViewModel, TProps>>) {
    const renderComponent = (props: TProps) => {
        const params = useParams();
        const dialogMediatorContext = useRef<IDialogMediatorHandler | null>(null);
        const currentViewModel = useRef<TViewModel | null>(null);
        const [_, setInitialRender] = useState(true);
        const parentDialogMediator = useDialogMediator();

        dialogMediatorContext.current = useMemo(() => {
            return new DialogMediatorHandler(parentDialogMediator);
        }, []);

        useEffect(() => {
            const child = container.createChildContainer();
            child.registerInstance('props', props);
            child.registerInstance('params', params);

            const dialogService = new Dialogs(dialogMediatorContext.current!);
            child.registerInstance<IDialogs>(IDialogs as Constructor<IDialogs>, dialogService);
            const viewModel = child.resolve<TViewModel>(viewModelType) as any;
            makeAutoObservable(viewModel);
            viewModel.__childContainer = child;
            viewModel.__magic = Guid.create();

            currentViewModel.current = viewModel;

            setInitialRender(false);

            return () => {
                disposeViewModel(currentViewModel.current);
            };
        }, []);

        if (currentViewModel.current === null) return null;

        const component = () => targetComponent({ viewModel: currentViewModel.current!, props }) as ReactElement<any, string>;

        return (
            <DialogMediator handler={dialogMediatorContext.current!}>
                <Observer>
                    {component}
                </Observer>
            </DialogMediator>
        );
    };

    return renderComponent;
}
