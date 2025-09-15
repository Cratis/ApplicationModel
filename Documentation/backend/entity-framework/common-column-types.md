# Common Column Types

One of the key design goals of the Cratis Application Model support for Entity Framework is to make it easy to support
different databases for an application.

If you're willing to hand-roll Entity Framework migrations, you can leverage the extension methods that will give you
a single migration but support different database types resolved at runtime.

## Auto Incremental

Auto incremental primary keys are very common but implemented in different ways with different annotations for them to
actual auto increment. The `.AutoIncrement()` extension method for the the `ColumnsBuilder` give you a way to configure
it once.

Below shows an example of how this can be used.

```csharp
[DbContext(typeof(EventLogDbContext))]
[Migration($"EventLog_{nameof(v1_0_0)}")]
public class v1_0_0 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventLog",
            columns: table => new
            {
                SequenceNumber = table.AutoIncrement(migrationBuilder), // Creates an auto increment column of correct integer type
                /*
                Other columns...
                */
            },
            constraints: table => table.PrimaryKey("PK_EventLog", x => x.SequenceNumber));
    }
}
```
