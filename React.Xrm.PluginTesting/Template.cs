using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
// using Example.CRM.Plugins.<Entity>;

namespace React.Xrm.PluginTesting
{
    class Program
    {
        private static CrmServiceClient _client;

        /// <summary>
        /// Example of how to use the PluginTesting package in a D365 Console Application.<br/>
        /// A reference to the Plugin Assembly is required.<br/>
        /// Generating the local Execution context for a Plugin requires the SDK Message name, the Primary Entity type and Guid for the target record to be tested. null or string.Empty can be passed for Global Actions.<br/>
        /// The non-static Plugin Execute method requires a Plugin intance to be created, passing secure/unsecure configs as parameters if needed.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                using (_client = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString))
                {
                    Guid id = new Guid("00000000-0000-0000-0000-000000000000");

                    var context = ExecutionContextFactory.Create("{sdk_message_name}", "{primary_entity_type}", id, _client);
                    context.InputParameters["Target"] = new Entity("{primary_entity_type}", id);
                    // add Input Parameters...

                    var serviceProvider = new LocalServiceProvider(_client, context);

                    // create plugin instance...
                    // plugin.Execute(serviceProvider);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                string message = ex.Message;
                throw;
            }
        }
    }
}
