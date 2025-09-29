// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.ModelBound.for_ModelBoundQueryPerformer.when_performing;

public class with_null_argument_for_non_nullable_parameter : given.a_model_bound_query_performer
{
    public record TestReadModel
    {
        public static TestReadModel Query(int requiredInt, string requiredString)
        {
            return new TestReadModel();
        }
    }

    MissingArgumentForQuery _exception;

    void Establish()
    {
        var parameters = new QueryArguments
        {
            ["requiredString"] = "valid string"

            // requiredInt is intentionally missing, which will result in a null value
        };

        EstablishPerformer<TestReadModel>(nameof(TestReadModel.Query), parameters: parameters);
    }

    async Task Because() => _exception = await Catch.Exception(PerformQuery) as MissingArgumentForQuery;

    [Fact] void should_throw_null_argument_for_non_nullable_parameter_exception() => _exception.ShouldNotBeNull();
    [Fact] void should_have_parameter_name_in_exception_message() => _exception.Message.ShouldContain("requiredInt");
    [Fact] void should_have_parameter_type_in_exception_message() => _exception.Message.ShouldContain("Int32");
    [Fact] void should_have_query_name_in_exception_message() => _exception.Message.ShouldContain("Query");
}