using Microsoft.Xrm.Sdk;
using System;

namespace Navicon.Plugins.Common
{
    public class PluginsHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOrganizationService _organizationService;
        private readonly ITracingService _traceService;

        public PluginsHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _organizationService = GetOrganizationService();
            _traceService = GetTraceService();
        }

        public IOrganizationService OrganizationService => _organizationService;
        public Entity TargetEntity => GetTarget();
        public EntityReference TargetEntityReference => GetReference();
        public ITracingService TracingService => _traceService;

        private IOrganizationService GetOrganizationService()
        {
            var serviceFacroty = (IOrganizationServiceFactory)_serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            return serviceFacroty.CreateOrganizationService(Guid.Empty);
        }

        private Entity GetTarget()
        {
            var pluginContext = (IPluginExecutionContext)_serviceProvider.GetService(typeof(IPluginExecutionContext));
            return (Entity)pluginContext.InputParameters["Target"];
        }

        private EntityReference GetReference()
        {
            var pluginContext = (IPluginExecutionContext)_serviceProvider.GetService(typeof(IPluginExecutionContext));
            return (EntityReference)pluginContext.InputParameters["Target"];
        }

        private ITracingService GetTraceService()
        {
            return (ITracingService)_serviceProvider.GetService(typeof(ITracingService));
        }

    }
}
