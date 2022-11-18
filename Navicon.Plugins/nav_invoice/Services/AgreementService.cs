using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Navicon.Plugins.nav_invoice.Services
{
    public class AgreementService
    {
        private readonly IOrganizationService _service;
        public AgreementService(IOrganizationService service )
        {
            _service = service;
        }

        /// <summary>
        /// Get Invoice Entity by target entity
        /// </summary>
        public Entity GetInvoice(Entity entity)
        {
            var invoice = _service.Retrieve("nav_invoice", entity.Id, new ColumnSet("nav_dogovorid", "nav_amount", "nav_fact"));
            return invoice;
        }

        /// <summary>
        /// Get Invoice Entity by target entityReference
        /// </summary>
        public Entity GetInvoice(EntityReference entity)
        {
            var invoice = _service.Retrieve("nav_invoice", entity.Id, new ColumnSet("nav_dogovorid", "nav_amount", "nav_fact"));
            return invoice;
        }

        /// <summary>
        /// Updates agreement sums 
        /// </summary>
        public void UpdateSum(Entity invoice, bool toDelete = false)
        {
            if (!invoice.Attributes.Contains("nav_dogovorid")) throw new Exception("Agreement is empty");
            if (!invoice.Attributes.Contains("nav_amount")) throw new Exception("Amount is empty");
            if (!invoice.Attributes.Contains("nav_fact")) throw new Exception("Fact is empty");

            var agreementId = invoice.GetAttributeValue<EntityReference>("nav_dogovorid").Id;
            if (invoice.GetAttributeValue<bool>("nav_fact"))
            {
                UpdateAgreementSums(agreementId, invoice.GetAttributeValue<Money>("nav_amount"), toDelete);
            }
        }

        private void UpdateAgreementSums(Guid agreementId, Money amount, bool toDelete = false)
        {
            var agreement = GetAgreementWithSums(agreementId);

            Entity agreemantToUpdate = new Entity("nav_agreement", agreementId);
            if (agreement.Attributes.Contains("nav_factsumma"))
            {
                if (toDelete)
                {
                    agreemantToUpdate["nav_factsumma"] = new Money(agreement.GetAttributeValue<Money>("nav_factsumma").Value - amount.Value);
                }
                else
                {
                    agreemantToUpdate["nav_factsumma"] = new Money(agreement.GetAttributeValue<Money>("nav_factsumma").Value + amount.Value);
                    CheckAgreementInvoicesSum(agreement, agreemantToUpdate);
                }
            }
            else
            {
                agreemantToUpdate["nav_factsumma"] = amount;
                CheckAgreementInvoicesSum(agreement, agreemantToUpdate);
            }

            _service.Update(agreemantToUpdate);

            CheckAgreementIsPaiedUpdate(agreementId);
        }

        private Entity GetAgreementWithSums(Guid agreementId)
        {
            var agreement = _service.Retrieve("nav_agreement", agreementId, new ColumnSet("nav_factsumma", "nav_summa"));
            if (agreement == null)
            {
                throw new Exception("Agreement is not exists");
            }

            return agreement;
        }

        /// <summary>
        /// Check that agreement's invoices sum less then agreement's sum
        /// </summary>
        private bool CheckAgreementInvoicesSum(Entity agreement, Entity agreemantToUpdate)
        {
            QueryExpression query = new QueryExpression("nav_invoice");
            query.ColumnSet = new ColumnSet("nav_amount");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_dogovorid", ConditionOperator.Equal, agreement.Id);
            query.Criteria.AddCondition("nav_fact", ConditionOperator.Equal, true);

            var result = _service.RetrieveMultiple(query);
            decimal invoiceSum = 0;

            foreach(var invoice in result.Entities)
            {
                invoiceSum += invoice.GetAttributeValue<Money>("nav_amount") != null ? invoice.GetAttributeValue<Money>("nav_amount").Value : 0;
            }
            
            if(invoiceSum > agreement.GetAttributeValue<Money>("nav_summa").Value)
            {
                throw new Exception("Amount of invoices is bigger then agreement's amount!");
            }
            else
            {
                UpdateAgreemnetDate(agreemantToUpdate);
            }

            return false;
        }

        /// <summary>
        /// Update Дата графика платежей field
        /// </summary>
        private void UpdateAgreemnetDate(Entity agreemantToUpdate)
        {
            agreemantToUpdate["nav_paymantplandate"] = DateTime.Now;
        }

        /// <summary>
        /// Check that agreement was paied 
        /// </summary>
        private void CheckAgreementIsPaiedUpdate(Guid agreementId)
        {
            var agreement = GetAgreementWithSums(agreementId);
            Entity agreemantToUpdate = new Entity("nav_agreement", agreement.Id);
            var agreementSum = agreement.GetAttributeValue<Money>("nav_summa");
            var factSum = agreement.GetAttributeValue<Money>("nav_factsumma");

            if(agreementSum != null && factSum != null && agreementSum.Value == factSum.Value)
            {
                UpdateAgreemnetIsPaied(agreemantToUpdate);
            }
        }

        /// <summary>
        /// Update Оплачено field
        /// </summary>
        private void UpdateAgreemnetIsPaied(Entity agreemantToUpdate)
        {
            agreemantToUpdate["nav_fact"] = true;
            _service.Update(agreemantToUpdate);
        }
    }
}
