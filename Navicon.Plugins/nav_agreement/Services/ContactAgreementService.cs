using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Navicon.Plugins.nav_agreement.Services
{
    public class ContactAgreementService
    {
        IOrganizationService _service;
        public ContactAgreementService(IOrganizationService service)
        {
            _service = service;
        }

        public void UpdateAgreemantDate(Entity agreement)
        {
            if (agreement.Attributes.Contains("nav_contactid"))
            {
                var contactId = agreement.GetAttributeValue<EntityReference>("nav_contactid").Id;
                Entity contactToUpdate = new Entity("contact", contactId);

                if (!agreement.Attributes.Contains("nav_date")) throw new Exception("Date is empty!");

                if(GetAgreemantsCount(contactId) == 1)
                {
                    contactToUpdate["nav_date"] = agreement.GetAttributeValue<DateTime>("nav_date");
                    _service.Update(contactToUpdate);
                }
            }
            else
            {
                throw new Exception("Contact is Empty!");
            }
        }

        /// <summary>
        /// Get count of agreement
        /// </summary>
        private int GetAgreemantsCount(Guid contactId)
        {
            QueryExpression query = new QueryExpression("nav_agreement");
            query.ColumnSet = new ColumnSet("nav_agreementid");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_contactid", ConditionOperator.Equal, contactId);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count();
        }
    }
}
