// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DialogResult } from './DialogResult';

export type DialogResponse<TResponse = {}> = [DialogResult, TResponse?];