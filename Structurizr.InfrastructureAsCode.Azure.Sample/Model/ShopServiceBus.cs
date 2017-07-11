using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopServiceBus : ContainerWithInfrastructure<ServiceBus>
    {
        public ShopServiceBus(Shop shop, IInfrastructureEnvironment environment)
        {
            Container = shop.System.AddContainer(
                "Shop Service Bus",
                "Provides the means for other services to communicate with each other",
                "Azure Service Bus");

            Infrastructure = new ServiceBus
            {
                Name = $"aac-sample-shop-servicebus-{environment.Name}",
                EnvironmentInvariantName = "aac-sample-shop-servicebus"
            };
        }
    }
}