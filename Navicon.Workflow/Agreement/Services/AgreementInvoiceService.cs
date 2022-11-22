using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navicon.Workflow.Agreement.Services
{
    public class AgreementInvoiceService
    {
        IOrganizationService _service;
        public AgreementInvoiceService(IOrganizationService service)
        {
            _service = service;
        }

        public bool IsExistsInvoice(Guid id)
        {
            QueryExpression query = new QueryExpression("nav_invoice");
            query.ColumnSet = new ColumnSet("nav_invoiceid");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, id);
            var results = _service.RetrieveMultiple(query);

            return results.Entities.Count() > 0 ? true : false;
        }

        public bool IsExistsPairdInvoice(Guid id)
        {
            QueryExpression query = new QueryExpression("nav_invoice");
            query.ColumnSet = new ColumnSet("nav_invoiceid");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, id);
            query.Criteria.AddCondition("nav_fact", ConditionOperator.Equal, true);

            var results = _service.RetrieveMultiple(query);

            return results.Entities.Count() > 0 ? true : false;
        }

        public bool IsExistsHandInvoice(Guid id)
        {
            QueryExpression query = new QueryExpression("nav_invoice");
            query.ColumnSet = new ColumnSet("nav_invoiceid");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, id);
            query.Criteria.AddCondition("nav_type", ConditionOperator.Equal, (int)InvoiceTypes.Hand);

            var results = _service.RetrieveMultiple(query);

            return results.Entities.Count() > 0 ? true : false;
        }

        public void DeleteInvoices(Guid id)
        {
            QueryExpression query = new QueryExpression("nav_invoice");
            query.ColumnSet = new ColumnSet("nav_invoiceid");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, id);
            query.Criteria.AddCondition("nav_type", ConditionOperator.Equal, (int)InvoiceTypes.Auto);

            var results = _service.RetrieveMultiple(query);

            foreach(var entity in results.Entities)
            {
                _service.Delete("nav_invoice", entity.Id);
            }
        }

        public void CreateInvoices(Guid id)
        {
            var agreement = _service.Retrieve("nav_agreement", id, new ColumnSet("nav_name","nav_creditperiod", "nav_creditamount"));

            var periodInMonths = agreement.GetAttributeValue<int>("nav_creditperiod") != 0 ? agreement.GetAttributeValue<int>("nav_creditperiod") * 12 : 0;
            var monthlySum = agreement.GetAttributeValue<Money>("nav_creditamount").Value / periodInMonths;

            for(int i = 1; i<=periodInMonths; i++)
            {
                Entity invoice = new Entity("nav_invoice");
                invoice.Attributes.Add("nav_amount", new Money(monthlySum));
                invoice.Attributes.Add("nav_dogovorid", new EntityReference("nav_agreement", id));
                invoice.Attributes.Add("nav_name", $"Счет на оплату по договору {agreement.GetAttributeValue<string>("nav_name")}");
                invoice.Attributes.Add("nav_date", DateTime.Now.AddMonths(i));
                invoice.Attributes.Add("nav_type", new OptionSetValue((int)InvoiceTypes.Auto));
                _service.Create(invoice);
            }
        }

        public void UpdateAgreementDate(Guid id, DateTime date)
        {
            Entity agreemantToUpdate = new Entity("nav_agreement", id);
            agreemantToUpdate["nav_paymantplandate"] = date;
            _service.Update(agreemantToUpdate);
        }
    }
}
