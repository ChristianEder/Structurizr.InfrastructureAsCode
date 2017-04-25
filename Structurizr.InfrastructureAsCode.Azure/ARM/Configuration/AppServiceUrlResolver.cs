using System;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class AppServiceUrlResolver : ConfigurationValueResolver<AppServiceUrl>
    {
        private readonly AzureConfigurationValueResolverContext _context;
        public AppServiceUrlResolver(AzureConfigurationValueResolverContext context)
        {
            _context = context;
        }

        public override bool CanResolve(AppServiceUrl value)
        {
            return true;
        }

        public override async Task<object> Resolve(AppServiceUrl value)
        {
           var webApp = await _context.Azure.AppServices.WebApps.GetByGroupAsync(_context.ResourceGroupName, value.AppService.Name);
            return webApp.DefaultHostName;
        }
    }
}