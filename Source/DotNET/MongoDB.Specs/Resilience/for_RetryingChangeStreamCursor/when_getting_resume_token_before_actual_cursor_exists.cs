// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;

namespace Cratis.Arc.MongoDB.Resilience.for_RetryingChangeStreamCursor;

public class when_getting_resume_token_before_actual_cursor_exists : given.a_retrying_change_stream_cursor
{
    BsonDocument? _result;

    void Because() => _result = _cursor.GetResumeToken();

    [Fact] void should_return_null() => _result.ShouldBeNull();
}
