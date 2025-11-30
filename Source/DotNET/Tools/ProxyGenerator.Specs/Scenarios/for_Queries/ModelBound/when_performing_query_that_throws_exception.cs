// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;
using Cratis.Arc.ProxyGenerator.Scenarios.Queries;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Queries.ModelBound;

public class when_performing_query_that_throws_exception : given.a_scenario_web_application
{
    QueryExecutionResult<ExceptionReadModel>? _executionResult;

    async Task Because()
    {
        var parameters = new Dictionary<string, object>
        {
            ["shouldThrow"] = true
        };

        // ExceptionReadModel.GetWithException
        _executionResult = await Bridge!.PerformQueryDirectAsync<ExceptionReadModel>(
            "/api/cratis/arc/proxy-generator/scenarios/queries/get-with-exception",
            parameters);
    }

    [Fact] void should_not_be_successful() => _executionResult!.Result.IsSuccess.ShouldBeFalse();
    [Fact] void should_have_exceptions() => _executionResult!.Result.HasExceptions.ShouldBeTrue();
    [Fact] void should_have_exception_messages() => _executionResult!.Result.ExceptionMessages.ShouldNotBeEmpty();
}
