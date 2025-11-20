// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_command } from '../given/a_command';
import { given } from '../../../given';
import { CommandResult } from '../../CommandResult';

describe("when executing and fetch throws exception", given(a_command, context => {
    let result: CommandResult<object>;

    beforeEach(async () => {
        context.fetchStub.rejects(new Error('Network error'));

        result = await context.command.execute();
    });

    afterEach(() => {
        context.fetchStub.restore();
    });

    it("should_return_failed_result", () => result.isSuccess.should.be.false);
    it("should_include_error_message", () => result.exceptionMessages[0].should.contain('Error during server call'));
    it("should_have_exception_messages", () => (result.exceptionMessages.length > 0).should.be.true);
}));
