// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { FC } from 'react';
import { ActualDialogProps } from './ActualDialogProps';

export type WrappedDialogComponent<T> = FC<ActualDialogProps<T>>;
