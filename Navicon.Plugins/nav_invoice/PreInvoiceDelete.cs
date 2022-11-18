using Microsoft.Xrm.Sdk;
using Navicon.Plugins.Common;
using Navicon.Plugins.nav_invoice.Services;
using System;

namespace Navicon.Plugins.nav_invoice
{
    public sealed class PreInvoiceDelete : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            PluginsHandler pluginHandler = new PluginsHandler(serviceProvider);
            var traceService = pluginHandler.TracingService;
            var invoice = pluginHandler.TargetEntityReference;
            var service = pluginHandler.OrganizationService;

            try
            {
                AgreementService agreementService = new AgreementService(service);
                var invoiceEntity = agreementService.GetInvoice(invoice);
                agreementService.UpdateSum(invoiceEntity, true);
            }
            catch (Exception ex)
            {
                traceService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
