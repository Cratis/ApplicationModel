# Scenarios

Add a folder to ProxyGenerator.Specs called `Scenarios`.
We want to enable the generation of proxy objects that are end to end. This means involving testing them from JavaScript to a backend.
Leverage Microsoft ASP.NET Core testing to set up the environment.
For the JavaScript environment, base it on Microsoft ClearScript with the Typescript transpiler running in memory.

Example of use:

```csharp
using (var engine = new V8ScriptEngine())
{
    // 1. Load TypeScript compiler
    engine.Execute(File.ReadAllText("typescript.js"));

    // 2. Transpile TS to JS
    var tsCode = "const x: number = 42;";
    var jsCode = engine.Script.ts.transpile(tsCode);

    // 3. Execute resulting JS
    engine.Execute(jsCode);
}
```

We don't need to generate from assemblies and to a file that is persisted on disk. We should be able to everything in memory.
The backend however needs all the scenarios hatched out. We want to have scenarios for both ASP.NET Core controller based and
the Model Bound approach. This means that the specs needs a minimum setup that involves having parts of Arc running, like
the Command and Query Action Filters and endpoints for model bound Commands and Queries.

We want to verify that proxies represent what the backend exposes by invoking the different generated proxies from the
different scenarios. We can then assert if values are coming across and that return values come back to JavaScript as expected.

Examples of scenarios, but not limited to:

- Commands
   - Valid
   - With validation errors
   - With exceptions
   - Not authorized
   - Content / Body of the command - properties are transferred
   - Result from commands are returned. For Model bound this is extra interesting as we have conventions for resolving what we suspect is the return type
   - For Controller based - check with things like [FromQuery] support
- Queries
   - Valid
   - With validation errors
   - With exceptions
   - Not authorized
   - Query arguments

You can look at existing specs for all of this to figure out what things we support. And don't hold back.
We want this to be rock solid. If you see scenarios the backend supports, but the proxy generator might not - implement specs that fail and fix the problems.
