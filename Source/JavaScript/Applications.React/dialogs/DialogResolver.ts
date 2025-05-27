// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/**
 * Represents the delegate when a dialog is resolved (closed with a response).
 */
export type DialogResolver<TResponse> = (response: TResponse) => void;
