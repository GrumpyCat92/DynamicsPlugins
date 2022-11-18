using Microsoft.Xrm.Sdk;
using Navicon.Plugins.Common;
using Navicon.Plugins.nav_agreement.Services;
using System;

namespace Navicon.Plugins.nav_agreement
{
    public sealed class PostAgreementCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            PluginsHandler pluginHandler = new PluginsHandler(serviceProvider);
            var traceService = pluginHandler.TracingService;
            var agreement = pluginHandler.TargetEntity;
            var service = pluginHandler.OrganizationService;

            try
            {
                ContactAgreementService agreementService = new ContactAgreementService(service);
                agreementService.UpdateAgreemantDate(agreement);
            }
            catch (Exception ex)
            {
                traceService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
