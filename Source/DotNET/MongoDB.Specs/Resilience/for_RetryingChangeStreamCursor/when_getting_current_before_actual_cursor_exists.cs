// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_RetryingChangeStreamCursor;

public class when_getting_current_before_actual_cursor_exists : given.a_retrying_change_stream_cursor
{
    IEnumerable<string> _result;

    void Because() => _result = _cursor.Current;

    [Fact] void should_return_empty_collection() => _result.ShouldBeEmpty();
}
