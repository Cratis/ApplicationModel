// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;
using Cratis.Arc.ProxyGenerator.Scenarios.Queries;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Queries.ModelBound;

public class when_performing_query_with_parameters : given.a_scenario_web_application
{
    QueryExecutionResult<IEnumerable<ParameterizedReadModel>>? _executionResult;

    async Task Because()
    {
        var parameters = new Dictionary<string, object>
        {
            ["name"] = "TestName",
            ["category"] = "TestCategory"
        };

        // ParameterizedReadModel.Search
        _executionResult = await Bridge!.PerformQueryDirectAsync<IEnumerable<ParameterizedReadModel>>(
            "/api/cratis/arc/proxy-generator/scenarios/queries/search",
            parameters);
    }

    [Fact] void should_return_successful_result() => _executionResult!.Result.IsSuccess.ShouldBeTrue();
    [Fact] void should_be_authorized() => _executionResult!.Result.IsAuthorized.ShouldBeTrue();
    [Fact] void should_have_data() => _executionResult!.Result.Data.ShouldNotBeNull();
}
