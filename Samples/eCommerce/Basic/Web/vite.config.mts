// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { defineConfig } from 'vite';
import react from "@vitejs/plugin-react";
import path from 'path';
import { EmitMetadataPlugin } from '@cratis/applications.vite';

export default defineConfig({
    build: {
        outDir: './wwwroot',
        assetsDir: '',
        rollupOptions: {
            external: [
            ],
        },
    },
    plugins: [
        react(),
        EmitMetadataPlugin() as any
    ],
    server: {
        port: 9000,
        open: false,
        proxy: {
            '/api': {
                target: 'http://localhost:5001',
                ws: true
            },
            '/swagger': {
                target: 'http://localhost:5001'
            },
            '/.cratis': {
                target: 'http://localhost:5001'
            }
        }
    },
    resolve: {
        alias: {
            'API': path.resolve('./API'),
            'assets': path.resolve('./assets'),
            'Components': path.resolve('./Components'),
            'Strings': path.resolve('./Strings')
        }
    }
});
