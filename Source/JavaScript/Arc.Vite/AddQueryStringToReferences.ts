// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export const AddQueryStringToReferences = (queryString: string) => {
    return {
        name: 'add-query-string-to-references',
        enforce: 'post', // Ensure it runs after the default processes
        generateBundle(options, bundle) {
            for (const file in bundle) {
                const chunk = bundle[file];
                if (chunk.type === 'chunk') {
                    chunk.code = chunk.code.replace(/(import\s+.*?from\s+['"])(.*?)(['"])/g, `$1$2?${queryString}$3`);
                    chunk.code = chunk.code.replace(/(export\s+.*?from\s+['"])(.*?)(['"])/g, `$1$2?${queryString}$3`);
                }

                if (chunk.type === 'asset' && chunk.fileName.endsWith('.css')) {
                    // Modify CSS url() references
                    chunk.source = chunk.source.replace(/(url\(\/['"]?)(.*?)(['"]?\))/g, `$1$2?${queryString}$3`);
                }
            }
        },
    };
};
