// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Sorting } from '@cratis/applications/queries';

/**
 * Delegate type for setting the sorting in the context of the {@link useQuery}, {@link useQueryWithPaging}, {@link useObservableQuery} or {@link useObservableQueryWithPaging} hooks.
 */
export type SetSorting = (sorting: Sorting) => Promise<void>;
