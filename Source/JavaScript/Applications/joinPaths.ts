// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export function joinPaths(...paths) {
    const joined = paths.join('/');
    return joined.replace('//', '/');
}