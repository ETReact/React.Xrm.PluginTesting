using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace React.Xrm.PluginTesting
{
    /// <summary>
    /// Implements a Constructor that receives the IOrganizationService containing the connection to CRM via Connection String, and a local instance of the ExectionContext.<br/>
    /// Provides the IExecutionContext and local implementations of ITracingService and IOrganizationServiceFactory to execute the Plugin.<br/>
    /// Initializing a new instance of this class allows to call the "Execute" method of a basic D365 Plugin, using the object as the required parameter "serviceProvider".
    /// </summary>
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

    /// <summary>
    /// Creates a Local-based Tracing Service to be passed in the serviceProvider parameter of the Execute method: transposes the ITracingService behaviour in a local Console visualization of Plugin Trace Logs.
    /// </summary>
    public class ConsoleTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
            => System.Console.WriteLine(args != null && args.Length > 0 ? string.Format(format, args) : format);
    }

    /// <summary>
    /// Creates a Local-based Service Factory to be passed in the serviceProvider parameter of the Execute method: provides the IOrganizationServiceFactory component.
    /// </summary>
    public class LocalServiceFactory : IOrganizationServiceFactory
    {
        private readonly IOrganizationService _service;
        public LocalServiceFactory(IOrganizationService service) { _service = service; }

        public IOrganizationService CreateOrganizationService(Guid? userId) => _service;
    }
}
