using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Navicon.Workflow.Agreement.Services;
using Navicon.Workflow.WFCommon;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navicon.Workflow.Agreement
{
    public class AgreementInvoiceDateUpdating : CodeActivity
    {
        [Input("Agreement")]
        [RequiredArgument]
        [ReferenceTarget("nav_agreement")]
        public InArgument<EntityReference> AgreementReference { get; set; }

        [Input("Date of agreement payment")]
        [RequiredArgument]
        public InArgument<DateTime> PaiedDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var handler = new WFHandler(context);
            var service = handler.OrganizationService;
            var agreementService = new AgreementInvoiceService(service);

            var agreement = AgreementReference.Get(context);
            var paiedDate = PaiedDate.Get(context);
            agreementService.UpdateAgreementDate(agreement.Id, paiedDate);
        }
    }
}
