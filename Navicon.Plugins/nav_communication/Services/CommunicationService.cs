using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Navicon.Plugins.nav_communication.Services
{
    public class CommunicationService
    {
        IOrganizationService _service;
        public CommunicationService(IOrganizationService service)
        {
            _service = service;
        }

       /// <summary>
       /// Get communication by entity
       /// </summary>
        public Entity GetCommunication(Entity entity)
        {
            var invoice = _service.Retrieve("nav_communication", entity.Id, new ColumnSet("nav_contactid", "nav_type", "nav_main"));
            return invoice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="communication">Communication entity</param>
        /// <param name="isUpdated">Mark that it's update plugin</param>
        public void CheckMainContact(Entity communication, bool isUpdated = false)
        {
            if (!communication.Attributes.Contains("nav_contactid")) throw new Exception("Contact is empty");
            if (!communication.Attributes.Contains("nav_type")) throw new Exception("Type is empty");

            var contactId = communication.GetAttributeValue<EntityReference>("nav_contactid").Id;
            var typeOfCommunication = communication.GetAttributeValue<OptionSetValue>("nav_type");

            Guid communicationId = isUpdated ? communication.Id : Guid.Empty;
            int count = GetCommunicationCount(contactId, typeOfCommunication, communicationId);

            if(count > 0)
            {
                throw new Exception($"Main communication for contact with this type exists");
            }
        }

        /// <summary>
        /// Get count of communication records
        /// </summary>
        /// <returns></returns>
        private int GetCommunicationCount(Guid contactId, OptionSetValue option, Guid communicationId)
        {
            QueryExpression query = new QueryExpression("nav_communication");
            query.ColumnSet = new ColumnSet("nav_name");
            query.NoLock = true;
            query.Criteria.AddCondition("nav_contactid", ConditionOperator.Equal, contactId);
            query.Criteria.AddCondition("nav_main", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("nav_type", ConditionOperator.Equal, option.Value);
            if (communicationId != Guid.Empty)
            {
                query.Criteria.AddCondition("nav_communicationid", ConditionOperator.NotEqual, communicationId);
            }

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Count();
        }
    }
}
