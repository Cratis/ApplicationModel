// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { container, DependencyContainer } from "tsyringe";
import { Constructor } from '@cratis/fundamentals';
import { FunctionComponent, ReactElement, useEffect, useMemo, useRef } from 'react';
import { Observer } from 'mobx-react';
import { makeAutoObservable } from 'mobx';
import { useParams } from 'react-router-dom';
import {
    DialogMediator,
    DialogMediatorHandler,
    Dialogs,
    IDialogMediatorHandler,
    IDialogs,
    useDialogMediator
} from './dialogs';
import { IViewModelDetached } from './dialogs/IViewModelDetached';

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
        const vm = useRef<TViewModel | null>(null);
        const parentDialogMediator = useDialogMediator();

        dialogMediatorContext.current = useMemo(() => {
            return new DialogMediatorHandler(parentDialogMediator);
        }, []);

        vm.current = useMemo(() => {
            const child = container.createChildContainer();

            child.registerInstance('props', props);
            child.registerInstance('params', params);

            const dialogService = new Dialogs(dialogMediatorContext.current!);
            child.registerInstance<IDialogs>(IDialogs as Constructor<IDialogs>, dialogService);
            const vm = child.resolve<TViewModel>(viewModelType) as any;
            makeAutoObservable(vm);
            vm.__childContainer = child;
            return vm;
        }, []);

        useEffect(() => {
            return () => {
                const currentVm = vm.current as any;
                if (!currentVm) {
                    return;
                }

                const vmWithDetach = (currentVm as IViewModelDetached);
                if (typeof (vmWithDetach.detached) == 'function') {
                    vmWithDetach.detached();
                }

                if (currentVm.__childContainer) {
                    const container = currentVm.__childContainer as DependencyContainer;
                    container.dispose();
                }
            };
        }, []);

        const component = () => targetComponent({ viewModel: vm.current!, props }) as ReactElement<any, string>;
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
