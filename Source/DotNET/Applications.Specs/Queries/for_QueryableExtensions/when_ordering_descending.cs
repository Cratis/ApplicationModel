// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Queries.for_QueryableExtensions;

public class when_ordering_descending : Specification
{
    record item(int Value);

    IQueryable queryable;
    item[] actual_collection = [new(1), new(2), new(3), new(4)];
    item[] result;

    void Establish() => queryable = actual_collection.AsQueryable();

    void Because() => result = [.. queryable.OrderByDescending(nameof(item.Value)).Cast<item>()];

    [Fact] void should_be_ordered_correctly() => result.ShouldEqual(actual_collection.OrderByDescending(_ => _.Value));
}
