# Response Value Handlers

For values returned from a command handler to have any impact, there needs to be a
value handler for it.

Out-of-the-box the Cratis ApplicationModel comes with the following

| Type | Description |
| ---- | ----------- |
| ValidationResultResponseValueHandler | Responds to Cratis ValidationResult object and adds it to the command result |

## Creating your own

You can easily create your own by simply adding a class to your solution that implements the interface
`ICommandResponseValueHandler`.

The Cratis ApplicationModel will automatically discover it and use it in the command pipeline automatically.
With this interface you get need to implement `CanHandle`, which is called to figure out
if the value in the context of a command can be handled the handler. If it returns true,
the `Handle` method is then called. This method can then perform operations as it see fit and
also return a result which will be merged with result of any other parts of the command pipeline
and used as the result.