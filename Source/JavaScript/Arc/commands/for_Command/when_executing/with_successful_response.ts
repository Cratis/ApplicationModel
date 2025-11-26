// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { a_command } from '../given/a_command';
import { given } from '../../../given';
import { CommandResult } from '../../CommandResult';

describe("when executing with successful response", given(a_command, context => {
    let result: CommandResult<object>;
    const responseData = {
        correlationId: '12345678-1234-1234-1234-123456789012',
        isSuccess: true,
        isAuthorized: true,
        isValid: true,
        hasExceptions: false,
        validationResults: [],
        exceptionMessages: [],
        exceptionStackTrace: '',
        response: { data: 'test' }
    };

    beforeEach(async () => {
        context.fetchStub.resolves({
            status: 200,
            json: async () => responseData
        });

        result = await context.command.execute();
    });

    afterEach(() => {
        context.fetchStub.restore();
    });

    it("should_call_fetch_with_correct_url", () => context.fetchStub.calledOnce.should.be.true);

    it("should_call_fetch_with_post_method", () => {
        const call = context.fetchStub.getCall(0);
        call.args[1].method.should.equal('POST');
    });

    it("should_call_fetch_with_json_headers", () => {
        const call = context.fetchStub.getCall(0);
        call.args[1].headers['Content-Type'].should.equal('application/json');
        call.args[1].headers['Accept'].should.equal('application/json');
    });

    it("should_return_command_result", () => (result !== null && result !== undefined).should.be.true);

    it("should_set_initial_values_from_current_values", () => context.command.hasChanges.should.be.false);
}));
