// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { CloseDialog } from './CloseDialog';

export interface DialogProps<TResponse = {}> {
    closeDialog: CloseDialog<TResponse>;
}

