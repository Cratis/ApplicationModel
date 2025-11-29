// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export const deepEqual = (obj1, obj2) => {
    if (obj1 === obj2) return true; // Same reference or primitive value

    if (obj1 == null || obj2 == null || typeof obj1 !== "object" || typeof obj2 !== "object") {
        return false; // Handle null, undefined, or different types
    }

    const keys1 = Object.keys(obj1);
    const keys2 = Object.keys(obj2);

    if (keys1.length !== keys2.length) {
        return false; // Different number of keys
    }

    for (const key of keys1) {
        if (!keys2.includes(key) || !deepEqual(obj1[key], obj2[key])) return false;
    }

    return true; // All keys and values match
};