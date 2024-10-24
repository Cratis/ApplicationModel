// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
import { withViewModel } from "@cratis/applications.react.mvvm";
import { TestParamsViewModel } from "./TestParamsViewModel";


export type Params = { 
    id?: string; 
    num?: number;
};

export const TestParams = withViewModel(TestParamsViewModel, ({ viewModel }) => {
    return (
        <h1>Hello params</h1>
    );
});
