// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResponse } from './DialogResponse';

export type ShowDialog<TProps, TResponse = {}> = (props?: TProps) => Promise<DialogResponse<TResponse>>;