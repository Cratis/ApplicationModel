// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_RetryingChangeStreamCursor;

public class when_moving_next_with_cancellation : given.a_retrying_change_stream_cursor
{
    CancellationTokenSource _cancellationTokenSource;
    Exception? _exception;

    void Establish()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSource.Cancel();
    }

    async Task Because()
    {
        _exception = await Catch.Exception(async () => await _cursor.MoveNextAsync(_cancellationTokenSource.Token));
    }

    [Fact] void should_throw_operation_canceled_exception() => _exception.ShouldBeOfExactType<OperationCanceledException>();
}
