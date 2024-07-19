// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryableExtensions;

public class when_counting : Specification
{
    IQueryable queryable;
    int[] actual_collection = [1, 2, 3, 4];
    int result;

    void Establish() => queryable = actual_collection.AsQueryable();

    void Because() => result = queryable.Count();

    [Fact] void should_have_same_count() => result.ShouldEqual(actual_collection.Length);
}
