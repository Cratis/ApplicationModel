/// <reference types="vitest/config" />
import commonjs from 'vite-plugin-commonjs';
import tsconfigPaths from 'vite-tsconfig-paths';

const cwd = process.cwd();

export function createConfig() {
    return {
        optimizeDeps: {
            exclude: ['tslib'],
        },
        esbuild: {
            supported: {
                'top-level-await': true,
            },
        },
        test: {
            globals: true,
            environment: 'node',
            sourcemap: false,
            isolate: false,
            fileParallelism: false,
            pool: 'threads',
            poolOptions: {
                forks: {
                    isolate: false,
                },
            },
            mock: {
                exclude: ['**/node_modules/**', 'node_modules/**'],
            },
            coverage: {
                exclude: [
                    '**/for_*/**',
                    '**/wwwroot/**',
                    '**/api/**',
                    '**/Api/**',
                    '**/dist/**',
                    '**/*.test.tsx',
                    '**/*.d.ts',
                    '**/declarations.ts',
                ],
            },
            exclude: ['**/dist/**', '**/node_modules/**', 'node_modules/**', '**/wwwroot/**', 'wwwroot/**'],
            include: ['**/for_*/when_*/**/*.ts', '**/for_*/**/when_*.ts'],
            setupFiles: `${__dirname}/vitest.setup.ts`
        },
        plugins: [
            commonjs(),
            tsconfigPaths()
        ]
    };
}
