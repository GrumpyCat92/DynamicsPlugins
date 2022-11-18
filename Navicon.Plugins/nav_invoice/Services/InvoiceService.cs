using Microsoft.Xrm.Sdk;

namespace Navicon.Plugins.nav_invoice.Services
{
    public class InvoiceService
    {
        private readonly IOrganizationService _service;
        public InvoiceService(IOrganizationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Change type of invoice
        /// </summary>
        public void SetType(Entity invoice)
        {
            if(!invoice.Attributes.Contains("nav_type"))
            {
                Entity invoiceToUpdate = new Entity(invoice.LogicalName, invoice.Id);
                invoiceToUpdate["nav_type"] = new OptionSetValue((int)InvoiceTypes.Hand);
                _service.Update(invoiceToUpdate);
            }
        }
    }
}
