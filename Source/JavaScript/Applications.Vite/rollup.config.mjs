// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { rollup } from '../../../rollup.config.mjs';

import pkg from './package.json' with { type: 'json' };

import path from "path";

const cjsPath = path.dirname(pkg.main);
const esmPath = path.dirname(pkg.module);
const tsconfigPath = path.join(import.meta.dirname, "tsconfig.json");

export default rollup(cjsPath, esmPath, tsconfigPath, pkg);
