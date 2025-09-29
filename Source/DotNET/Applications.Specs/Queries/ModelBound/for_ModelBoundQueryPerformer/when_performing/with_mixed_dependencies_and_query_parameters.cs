// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.ModelBound.for_ModelBoundQueryPerformer.when_performing;

public class with_mixed_dependencies_and_query_parameters : given.a_model_bound_query_performer
{
    public record TestReadModel
    {
        public static object? ReceivedDependency { get; set; }
        public static string ReceivedName { get; set; } = string.Empty;
        public static int ReceivedAge { get; set; }

        public static TestReadModel Query(object dependency, string name, int age)
        {
            ReceivedDependency = dependency;
            ReceivedName = name;
            ReceivedAge = age;
            return new TestReadModel();
        }
    }

    object _expectedDependency = new();

    void Establish()
    {
        var parameters = new Dictionary<string, object>
        {
            ["name"] = "Mixed Example",
            ["age"] = "35"
        };

        var dependencies = new[] { _expectedDependency };

        EstablishPerformer<TestReadModel>(nameof(TestReadModel.Query), dependencies, parameters);
    }

    async Task Because() => await PerformQuery();

    [Fact] void should_inject_dependency() => TestReadModel.ReceivedDependency.ShouldEqual(_expectedDependency);
    [Fact] void should_match_name_from_query_string() => TestReadModel.ReceivedName.ShouldEqual("Mixed Example");
    [Fact] void should_match_age_from_query_string() => TestReadModel.ReceivedAge.ShouldEqual(35);
}