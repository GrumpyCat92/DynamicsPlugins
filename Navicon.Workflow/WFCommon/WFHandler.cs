using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navicon.Workflow.WFCommon
{
    public class WFHandler
    {
        CodeActivityContext _context;
        public WFHandler(CodeActivityContext context)
        {
            _context = context;
        }

        public IOrganizationService OrganizationService => GetOrganizationService();

        private IOrganizationService GetOrganizationService()
        {
            var factory = _context.GetExtension<IOrganizationServiceFactory>();
            return factory.CreateOrganizationService(null);
        }
    }
}
