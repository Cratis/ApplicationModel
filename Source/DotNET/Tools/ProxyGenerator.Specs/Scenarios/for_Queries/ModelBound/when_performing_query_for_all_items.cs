// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;
using Cratis.Arc.ProxyGenerator.Scenarios.Queries;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Queries.ModelBound;

public class when_performing_query_for_all_items : given.a_scenario_web_application
{
    QueryExecutionResult<IEnumerable<SimpleReadModel>>? _executionResult;

    void Establish() => LoadQueryProxy<SimpleReadModel>("GetAll");

    async Task Because()
    {
        _executionResult = await Bridge.PerformQueryViaProxyAsync<IEnumerable<SimpleReadModel>>("GetAll");
    }

    [Fact] void should_return_successful_result() => _executionResult.Result.IsSuccess.ShouldBeTrue();
    [Fact] void should_be_authorized() => _executionResult.Result.IsAuthorized.ShouldBeTrue();
    [Fact] void should_be_valid() => _executionResult.Result.IsValid.ShouldBeTrue();
    [Fact] void should_have_data() => _executionResult.Result.Data.ShouldNotBeNull();
}
