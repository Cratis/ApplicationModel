// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryPerformerProviders.when_constructing;

public class with_valid_providers : given.a_query_performer_providers
{
    void Because() => _queryPerformerProviders = new(new KnownInstancesOf<IQueryPerformerProvider>([_firstProvider, _secondProvider]));

    [Fact] void should_build_dictionary_with_all_performers() => _queryPerformerProviders.Performers.ShouldContain(_firstPerformer, _secondPerformer);
    [Fact] void should_make_performers_accessible_by_fully_qualified_name() => _queryPerformerProviders.TryGetPerformersFor(_firstPerformer.FullyQualifiedName, out _).ShouldBeTrue();
}