// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
import { withViewModel } from "@cratis/applications.react.mvvm";
import { TestQueryViewModel } from "./TestQueryViewModel";


export type Params = { 
    someString?: string; 
    num?: number;
};

export const TestQuery = withViewModel(TestQueryViewModel, ({ viewModel }) => {
    return (
        <h1>Hello</h1>
    );
});
