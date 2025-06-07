// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace Cratis.Applications.ModelBinding.for_FromRequestModelBinder;

public class when_model_is_provided_partially_by_both_binders : Specification
{
    FromRequestModelBinder binder;
    IModelBinder body_binder;
    IModelBinder complex_binder;

    ModelBindingContext context;
    ModelBindingResult result;

    ModelBindingResult final_result;

    TheModel body_model;
    TheModel complex_model;

    void Establish()
    {
        body_model = new TheModel(42, "forty two", 0, null!);
        complex_model = new TheModel(0, null!, 43, "forty three");

        context = Substitute.For<ModelBindingContext>();
        context.Result.Returns(_ => result);
        context.When(_ => _.Result = Arg.Any<ModelBindingResult>()).Do(_ => final_result = _.Arg<ModelBindingResult>());

        body_binder = Substitute.For<IModelBinder>();
        body_binder.BindModelAsync(context).Returns((_) =>
        {
            result = ModelBindingResult.Success(body_model);
            return Task.CompletedTask;
        });

        complex_binder = Substitute.For<IModelBinder>();
        complex_binder.BindModelAsync(context).Returns((_) =>
        {
            result = ModelBindingResult.Success(complex_model);
            return Task.CompletedTask;
        });

        binder = new(body_binder, complex_binder);
    }

    Task Because() => binder.BindModelAsync(context);

    [Fact] void should_hold_body_model_int() => ((TheModel)final_result.Model).intValue.ShouldEqual(body_model.intValue);
    [Fact] void should_hold_body_model_string() => ((TheModel)final_result.Model).stringValue.ShouldEqual(body_model.stringValue);
    [Fact] void should_hold_complex_model_int() => ((TheModel)final_result.Model).secondIntValue.ShouldEqual(complex_model.secondIntValue);
    [Fact] void should_hold_complex_model_string() => ((TheModel)final_result.Model).secondStringValue.ShouldEqual(complex_model.secondStringValue);
}
