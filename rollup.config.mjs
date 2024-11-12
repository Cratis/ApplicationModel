// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import typescript2 from 'rollup-plugin-typescript2';
import commonjs from 'rollup-plugin-commonjs';
import peerDepsExternal from 'rollup-plugin-peer-deps-external';

export function rollup(cjsPath, esmPath, tsconfigPath, pkg) {
    return {
        input: "index.ts",

        output: [
            {
                dir: cjsPath,
                format: "cjs",
                exports: "named",
                sourcemap: true,
                preserveModules: true,
                preserveModulesRoot: "."
            },
            {
                dir: esmPath,
                format: "es",
                exports: "named",
                sourcemap: true,
                preserveModules: true,
                preserveModulesRoot: "."
            }
        ],
        external: [
            ...Object.keys(pkg.dependencies || {}),
            ...Object.keys(pkg.peerDependencies || {}),
            /^@cratis\/applications/,
            'react',
            'react-dom',
        ],
        plugins: [
            peerDepsExternal(),
            commonjs({
                include: /node_modules/,
                esmExternals: true,
                namedExports: {
                    'react/jsx-runtime': ['tsx', 'jsx', 'jsxs'],
                },
            }),
            typescript2({
                exclude: "for_**/**/*",
                tsconfig: tsconfigPath,
                clean: true
            })
        ]
    };
}
