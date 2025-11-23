// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.MongoDB.Resilience.for_RetryingChangeStreamCursor;

public class when_disposing_cursor : given.a_retrying_change_stream_cursor
{
    void Establish()
    {
        _actualCursor.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
    }

    async Task Because()
    {
        _ = await _cursor.MoveNextAsync();
        _cursor.Dispose();
    }

    [Fact] void should_dispose_actual_cursor() => _actualCursor.Received(1).Dispose();
}
