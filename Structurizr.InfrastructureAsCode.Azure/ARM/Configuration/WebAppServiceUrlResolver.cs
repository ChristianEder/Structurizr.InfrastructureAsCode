using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class WebAppServiceUrlResolver : ConfigurationValueResolver<WebAppServiceUrl>
    {
        private readonly AzureConfigurationValueResolverContext _context;

        public WebAppServiceUrlResolver(AzureConfigurationValueResolverContext context)
        {
            _context = context;
        }

        public override bool CanResolve(WebAppServiceUrl value)
        {
            return true;
        }

        public override async Task<object> Resolve(WebAppServiceUrl value)
        {
            var webApp = await _context.Azure.WebApps.GetByResourceGroupAsync(_context.ResourceGroupName, value.AppService.Name);
            return webApp.DefaultHostName;
        }
    }
}