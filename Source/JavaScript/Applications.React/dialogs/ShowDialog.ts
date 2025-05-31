// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ActualDialogProps } from './ActualDialogProps';
import { DialogResponse } from './DialogResponse';

export type ShowDialog<TProps, TResponse = {}> = (props?: ActualDialogProps<TProps>) => Promise<DialogResponse<TResponse>>;