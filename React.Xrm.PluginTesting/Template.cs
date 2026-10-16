using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using Example.CRM.Plugins.Quote;

namespace React.Xrm.PluginTesting
{
    class Program
    {
        private static CrmServiceClient service;

        /// <summary>
        /// Example of how to use the PluginTesting package in a D365 Console Application.<br/>
        /// A reference to the Plugin Assembly is required.<br/>
        /// Generating the local Execution context for a Plugin requires the SDK Message name, the Primary Entity type and Guid for the target record to be tested. null or string.Empty can be passed for Global Actions.<br/>
        /// The non-static Plugin Execute method requires a Plugin intance to be created, passing secure/unsecure configs as parameters if needed.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            using (service = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString))
            {
                try
                {
                    Guid quoteId = new Guid("...");

                    var context = ExecutionContextFactory.Create("Update", "quote", quoteId, service);
                    context.InputParameters["Target"] = new Entity("quote", quoteId);

                    var serviceProvider = new LocalServiceProvider(service, context);

                    var plugin = new PostCreateQuote(null, null);
                    plugin.Execute(serviceProvider);

                }
                catch (Exception)
                {
                    System.Console.ReadKey();
                    throw;
                }
            }
        }
    }
}
