using Microsoft.Xrm.Sdk;
using Navicon.Plugins.Common;
using Navicon.Plugins.nav_communication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navicon.Plugins.nav_communication
{
    public sealed class PreValidationCommunicationUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            PluginsHandler pluginHandler = new PluginsHandler(serviceProvider);
            var traceService = pluginHandler.TracingService;
            var communication = pluginHandler.TargetEntity;
            var service = pluginHandler.OrganizationService;

            try
            {
                CommunicationService communicationService = new CommunicationService(service);
                var communicationToUpdae = communicationService.GetCommunication(communication);
                communicationService.CheckMainContact(communicationToUpdae, true);
            }
            catch (Exception ex)
            {
                traceService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
