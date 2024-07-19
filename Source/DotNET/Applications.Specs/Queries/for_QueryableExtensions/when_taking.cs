// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryableExtensions;

public class when_taking : Specification
{
    IQueryable queryable;
    int[] actual_collection = [1, 2, 3, 4];
    int[] result;

    void Establish() => queryable = actual_collection.AsQueryable();

    void Because() => result = [.. queryable.Take(2).Cast<int>()];

    [Fact] void should_contain_expected_items() => result.ShouldContainOnly(actual_collection.Take(2));
}