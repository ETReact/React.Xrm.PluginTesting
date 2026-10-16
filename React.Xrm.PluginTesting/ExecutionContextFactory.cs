using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace React.Xrm.PluginTesting
{
    /// <summary>
    /// Implements a Create method to initialize a Plugin context based on the SDK step that is going to be tested, and the Primary Entity that acts as the Target of the invocation.
    /// That allows to simulate the Plugin or the Action exposing the specified Input/Output Parameters.
    /// </summary>
    public class ExecutionContextFactory
    {
        /// <summary>
        /// Initialize a Plugin context based on the SDK step that is going to be tested, and the Primary Entity that acts as the Target of the invocation.
        /// </summary>
        /// <param name="actionName">The name of the SDK step that is going to be tested.</param>
        /// <param name="primaryEntityName">The Entity Type that acts as the Target of the invocation.</param>
        /// <param name="primaryEntityId">The Entity Id of the record that acts as the Target of the invocation.</param>
        /// <param name="service">The CrmServiceClient providing the connection to CRM.</param>
        /// <param name="isPreOperation">Optional: can simulate a Pre-operation or a Post-operation behaviour. Post-operation by default.</param>
        /// <returns></returns>
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
