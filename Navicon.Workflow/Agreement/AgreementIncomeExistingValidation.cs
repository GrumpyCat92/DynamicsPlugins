using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Navicon.Workflow.Agreement.Services;
using Navicon.Workflow.WFCommon;

namespace Navicon.Workflow.Agreement
{
    public class AgreementIncomeExistingValidation : CodeActivity
    {

        [Input("Agreement")]
        [RequiredArgument]
        [ReferenceTarget("nav_agreement")]
        public InArgument<EntityReference> AgreementReference { get; set; }

        [Output("Is an invoice exists")]
        public OutArgument<bool> IsExists { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var handler = new WFHandler(context);
            var service = handler.OrganizationService;
            var agreementService = new AgreementInvoiceService(service);

            var agreement = AgreementReference.Get(context);
            var isInvoiceExists = agreementService.IsExistsInvoice(agreement.Id);

            IsExists.Set(context, isInvoiceExists);
        }
    }
}
