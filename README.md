# React.Xrm.PluginTesting

Utility library for **running and debugging Dynamics 365 / Dataverse plugins locally**, from a console application, against a real CRM environment.

Instead of deploying a plugin and reading Plugin Trace Logs, you build a local execution context, wrap it in an `IServiceProvider`, and call the plugin's `Execute` method directly — with breakpoints, step-through debugging and trace output written to the console.

## Features

- **`ExecutionContextFactory`** – creates a `RemoteExecutionContext` for a given SDK message and target record (or a Global Action), simulating a Pre-operation or Post-operation stage.
- **`LocalServiceProvider`** – an `IServiceProvider` that exposes the services a plugin expects:
  - `IPluginExecutionContext` / `IExecutionContext`
  - `ITracingService` (via `ConsoleTracingService`)
  - `IOrganizationServiceFactory` (via `LocalServiceFactory`)
- **`ConsoleTracingService`** – writes `ITracingService.Trace(...)` calls to the console.
- **`LocalServiceFactory`** – returns the `IOrganizationService` you pass in (your `CrmServiceClient` connection).

## Requirements

- .NET Framework 4.6.2
- [`Microsoft.CrmSdk.CoreAssemblies`](https://www.nuget.org/packages/Microsoft.CrmSdk.CoreAssemblies) 9.0.2.60
- [`Microsoft.CrmSdk.XrmTooling.CoreAssembly`](https://www.nuget.org/packages/Microsoft.CrmSdk.XrmTooling.CoreAssembly) 9.1.1.103
- A reference to the plugin assembly you want to test

## Getting started

1. Create a **Console App (.NET Framework 4.6.2)**.
2. Add a reference to `React.Xrm.PluginTesting` (NuGet package or project reference) and to your plugin assembly.
3. Add a connection string to `App.config`:

   ```xml
   <configuration>
     <connectionStrings>
       <add name="CRMConnectionString"
            connectionString="AuthType=OAuth;Url=https://yourorg.crm.dynamics.com;..." />
     </connectionStrings>
   </configuration>
   ```

4. Build the context, create the service provider and execute the plugin:

   ```csharp
   using Microsoft.Xrm.Sdk;
   using Microsoft.Xrm.Tooling.Connector;
   using React.Xrm.PluginTesting;
   using System;
   using System.Configuration;

   class Program
   {
       static void Main(string[] args)
       {
           var connectionString = ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString;

           using (var service = new CrmServiceClient(connectionString))
           {
               Guid quoteId = new Guid("00000000-0000-0000-0000-000000000000");

               // 1. Local execution context for the "Update" message on a quote
               var context = ExecutionContextFactory.Create("Update", "quote", quoteId, service);
               context.InputParameters["Target"] = new Entity("quote", quoteId);

               // 2. Service provider exposing context, tracing and organization service
               var serviceProvider = new LocalServiceProvider(service, context);

               // 3. Instantiate the plugin (pass unsecure/secure config if needed) and run it
               var plugin = new MyQuotePlugin(null, null);
               plugin.Execute(serviceProvider);
           }
       }
   }
   ```

A complete example is available in [`Template.cs`](React.Xrm.PluginTesting/Template.cs) (excluded from compilation).

## API

### `ExecutionContextFactory.Create`

```csharp
RemoteExecutionContext Create(
    string actionName,
    string primaryEntityName,
    Guid primaryEntityId,
    CrmServiceClient service,
    bool isPreOperation = false)
```

| Parameter | Description |
|---|---|
| `actionName` | SDK message name (e.g. `Create`, `Update`, or a Custom Action name). |
| `primaryEntityName` | Logical name of the target entity. `null`, empty or `"none"` for Global Actions. |
| `primaryEntityId` | Id of the target record. Ignored (`Guid.Empty`) for Global Actions. |
| `service` | `CrmServiceClient` used to resolve the calling user (`UserId`). |
| `isPreOperation` | `true` → Stage 20 (Pre-operation); `false` → Stage 40 (Post-operation, default). |

The returned context has `Depth = 1`, `Mode = 0` (synchronous), a new `CorrelationId` and `OperationCreatedOn = DateTime.UtcNow`. Set any `InputParameters`, `OutputParameters`, `PreEntityImages` or `PostEntityImages` your plugin needs before executing it.

### `LocalServiceProvider`

```csharp
new LocalServiceProvider(IOrganizationService service, IPluginExecutionContext context)
```

Pass the instance as the `serviceProvider` argument of `IPlugin.Execute`. Service types other than those listed above return `null`.

## Testing Custom Actions

For Custom (or Global) Actions, use the action's unique name as the message and fill the input parameters manually:

```csharp
var context = ExecutionContextFactory.Create("new_MyGlobalAction", null, Guid.Empty, service);
context.InputParameters["MyInput"] = "value";

new MyActionPlugin().Execute(new LocalServiceProvider(service, context));

var result = context.OutputParameters["MyOutput"];
```

## Limitations

- The plugin runs against a **real environment**: any data it creates, updates or deletes is persisted.
- `IOrganizationServiceFactory.CreateOrganizationService(userId)` always returns the connection's service, so all calls run as the connected user regardless of the requested `userId`.
- No transaction/rollback, sandbox isolation or pipeline behaviour is simulated; images and parameters must be set up manually.

## License

See the repository for license information.
