using System.Linq;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public class ServiceBusConnectionStringResolver : ConfigurationValueResolver<ServiceBusConnectionString>
    {
        private readonly AzureConfigurationValueResolverContext _context;

        public ServiceBusConnectionStringResolver(AzureConfigurationValueResolverContext context)
        {
            _context = context;
        }

        public override bool CanResolve(ServiceBusConnectionString value)
        {
            return true;
        }

        public override async Task<object> Resolve(ServiceBusConnectionString value)
        {
            var serviceBus =
                await _context.Azure.ServiceBusNamespaces.GetByResourceGroupAsync(_context.ResourceGroupName,
                    value.ServiceBus.Name);

            var rules = await serviceBus.AuthorizationRules.ListAsync();
            var keys = await rules.First().GetKeysAsync();
            return keys.PrimaryConnectionString;
        }
    }
}