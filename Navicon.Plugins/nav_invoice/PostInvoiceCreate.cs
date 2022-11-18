using Microsoft.Xrm.Sdk;
using Navicon.Plugins.Common;
using Navicon.Plugins.nav_invoice.Services;
using System;

namespace Navicon.Plugins.nav_invoice
{
    public sealed class PostInvoiceCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            
            PluginsHandler pluginHandler = new PluginsHandler(serviceProvider);
            var traceService = pluginHandler.TracingService;
            var invoice = pluginHandler.TargetEntity;
            var service = pluginHandler.OrganizationService;

            try
            {

                InvoiceService invoiceService = new InvoiceService(service);
                AgreementService agreementService = new AgreementService(service);

                invoiceService.SetType(invoice);
                agreementService.UpdateSum(invoice);
            }
            catch (Exception ex)
            {
                traceService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
