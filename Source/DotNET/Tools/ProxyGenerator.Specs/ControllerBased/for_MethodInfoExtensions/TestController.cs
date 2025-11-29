// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_MethodInfoExtensions;

[Route("api/test")]
public class TestController
{
    [HttpGet]
    public Task<string> GetAll() => Task.FromResult("all");

    [HttpGet("{id}")]
    public Task<string> GetById([FromRoute] int id) => Task.FromResult($"item {id}");

    [HttpGet("search")]
    public Task<string> Search([FromQuery] string query) => Task.FromResult($"searching {query}");

    [HttpPost]
    public Task Create(string name) => Task.CompletedTask;

    [HttpGet]
    [AspNetResult]
    public Task<string> GetAspNetResult() => Task.FromResult("aspnet");
}
