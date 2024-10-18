// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import 'reflect-metadata';
import * as browser from './browser';
import * as messaging from './messaging';
import * as dialogs from './dialogs';
import { Bindings } from './Bindings';
import { MVVM, MVVMContext, MVVMProps } from './MVVMContext';
export * from './withViewModel';
export * from './IViewModelDetached';

export {
    Bindings,
    browser,
    messaging,
    dialogs,
    MVVM,
    MVVMContext,
    MVVMProps
};
