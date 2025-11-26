// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Delegate type for setting the page in the context of the {@link useQueryWithPaging} or {@link useObservableQueryWithPaging} hooks.
 */
export type SetPage = (page: number) => Promise<void>;
