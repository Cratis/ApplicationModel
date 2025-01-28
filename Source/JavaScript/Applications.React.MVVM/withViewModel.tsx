// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { container, DependencyContainer } from "tsyringe";
import { Constructor } from '@cratis/fundamentals';
import { FunctionComponent, ReactElement, useContext, useEffect, useRef, useState } from 'react';
import { Observer } from 'mobx-react';
import { makeAutoObservable } from 'mobx';
import { useParams, useSearchParams } from 'react-router-dom';
import {
    DialogMediator,
    DialogMediatorHandler,
    Dialogs,
    IDialogMediatorHandler,
    IDialogs,
    useDialogMediator
} from './dialogs';
import { IViewModelDetached } from './IViewModelDetached';
import { ApplicationModelContext } from '@cratis/applications.react';
import { WellKnownBindings } from "./WellKnownBindings";
import { deepEqual } from '@cratis/applications';
import { IHandleParams } from 'IHandleParams';

interface IViewModel extends IViewModelDetached {
    __childContainer: DependencyContainer;
}

function disposeViewModel(viewModel: IViewModel) {
    const vmWithDetach = (viewModel as IViewModelDetached);
    if (typeof (vmWithDetach.detached) == 'function') {
        vmWithDetach.detached();
    }

    if (viewModel.__childContainer) {
        const container = viewModel.__childContainer as DependencyContainer;
        container.dispose();
    }
}

function handleParams(viewModel: IViewModel, params: object) { 
    const vmWithHandleParams = (viewModel as unknown as IHandleParams);
    if (typeof (vmWithHandleParams.handleParams) == 'function') {
        vmWithHandleParams.handleParams(params);
    }
}

/**
 * Represents the view context that is passed to the view.
 */
export interface IViewContext<T, TProps = object> {
    viewModel: T,
    props: TProps,
}

/**
 * Use a view model with a component.
 * @param {Constructor} viewModelType View model type to use.
 * @param {FunctionComponent} targetComponent The target component to render.
 * @returns 
 */
export function withViewModel<TViewModel extends object, TProps extends object = object>(
    viewModelType: Constructor<TViewModel>,
    targetComponent: FunctionComponent<IViewContext<TViewModel, TProps>>) {

    const renderComponent = (props: TProps) => {
        const applicationContext = useContext(ApplicationModelContext);
        const params = useParams();
        const [previousParams, setPreviousParams] = useState(params);
        const [queryParams] = useSearchParams();
        const queryParamsObject = Object.fromEntries(queryParams.entries());
        const dialogMediatorContext = useRef<IDialogMediatorHandler | null>(null);
        const currentViewModel = useRef<TViewModel | null>(null);
        const [, setInitialRender] = useState(true);
        const parentDialogMediator = useDialogMediator();
        useEffect(() => {
            if (currentViewModel.current !== null) {
                return () => {
                    disposeViewModel(currentViewModel.current as IViewModel);
                };
            }

            dialogMediatorContext.current = new DialogMediatorHandler(parentDialogMediator);

            const child = container.createChildContainer();
            child.registerInstance(WellKnownBindings.props, props);
            child.registerInstance(WellKnownBindings.params, params);
            child.registerInstance(WellKnownBindings.queryParams, queryParamsObject);

            const dialogService = new Dialogs(dialogMediatorContext.current!);
            child.registerInstance<IDialogs>(IDialogs as Constructor<IDialogs>, dialogService);
            const viewModel = child.resolve<TViewModel>(viewModelType) as IViewModel;
            makeAutoObservable(viewModel);
            viewModel.__childContainer = child;
            currentViewModel.current = viewModel as TViewModel;

            setInitialRender(false);
            handleParams(viewModel, params);

            return () => {
                if (applicationContext.development === false) {
                    disposeViewModel(viewModel);
                }
            };
        }, []);

        if (currentViewModel.current === null) return null;

        if (!deepEqual(params, previousParams)) {
            setPreviousParams(params);
            handleParams(currentViewModel.current as IViewModel, params);
        }

        const component = () => targetComponent({ viewModel: currentViewModel.current!, props }) as ReactElement<object, string>;

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
