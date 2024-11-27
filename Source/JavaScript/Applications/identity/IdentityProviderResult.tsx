// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export type IdentityProviderResult = {
    id: string;
    name: string;
    claims: { [key: string]: string; };
    details: object;
};
