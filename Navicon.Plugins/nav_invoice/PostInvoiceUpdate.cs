using Microsoft.Xrm.Sdk;
using Navicon.Plugins.Common;
using Navicon.Plugins.nav_invoice.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navicon.Plugins.nav_invoice
{
    public sealed class PostInvoiceUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            PluginsHandler pluginHandler = new PluginsHandler(serviceProvider);
            var traceService = pluginHandler.TracingService;
            var invoice = pluginHandler.TargetEntity;
            var service = pluginHandler.OrganizationService;

            try
            {
                AgreementService agreementService = new AgreementService(service);
                var invoiceEntity = agreementService.GetInvoice(invoice);
                agreementService.UpdateSum(invoiceEntity);
            }
            catch (Exception ex)
            {
                traceService.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
