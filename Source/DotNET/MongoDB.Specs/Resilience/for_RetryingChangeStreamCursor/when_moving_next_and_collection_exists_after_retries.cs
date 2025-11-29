// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_RetryingChangeStreamCursor;

public class when_moving_next_and_collection_exists_after_retries : given.a_retrying_change_stream_cursor
{
    bool _result;

    void Establish()
    {
        _actualCursor.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
    }

    async Task Because() => _result = await _cursor.MoveNextAsync();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
    [Fact] void should_have_retried_multiple_times() => _target.CallCount.ShouldEqual(3);
    [Fact] void should_delegate_to_actual_cursor() => _actualCursor.Received(1).MoveNextAsync(Arg.Any<CancellationToken>());
}
