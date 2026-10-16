using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace React.Xrm.PluginTesting
{
    public class LocalServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public LocalServiceProvider(IOrganizationService service, IPluginExecutionContext context)
        {
            _services[typeof(ITracingService)] = new ConsoleTracingService();
            _services[typeof(IPluginExecutionContext)] = context;
            _services[typeof(IExecutionContext)] = context;
            _services[typeof(IOrganizationServiceFactory)] = new LocalServiceFactory(service);
        }

        public object GetService(Type serviceType)
            => _services.TryGetValue(serviceType, out var s) ? s : null;
    }

    public class ConsoleTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
            => System.Console.WriteLine(args != null && args.Length > 0 ? string.Format(format, args) : format);
    }

    public class LocalServiceFactory : IOrganizationServiceFactory
    {
        private readonly IOrganizationService _service;
        public LocalServiceFactory(IOrganizationService service) { _service = service; }

        public IOrganizationService CreateOrganizationService(Guid? userId) => _service;
    }

    public class ExecutionContextFactory
    {
        public static RemoteExecutionContext Create(string actionName, string primaryEntityName, Guid primaryEntityId, CrmServiceClient service, bool isPreOperation = false)
        {
            bool isGlobal = string.IsNullOrEmpty(primaryEntityName) || primaryEntityName == "none";

            return new RemoteExecutionContext
            {
                MessageName = actionName,
                PrimaryEntityName = isGlobal ? "none" : primaryEntityName,
                PrimaryEntityId = isGlobal ? Guid.Empty : primaryEntityId,
                Stage = isPreOperation ? 20 : 40,
                CorrelationId = Guid.NewGuid(),
                OperationCreatedOn = DateTime.UtcNow,
                UserId = service.GetMyCrmUserId(),
                Depth = 1,
                Mode = 0, // sync
            };
        }
    }
}
