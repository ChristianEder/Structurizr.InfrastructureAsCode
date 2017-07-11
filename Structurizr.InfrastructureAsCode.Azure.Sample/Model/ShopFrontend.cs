using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopFrontend : ContainerWithInfrastructure<AppService>
    {
        public ShopFrontend(Shop shop, ShopApi api, ShopServiceBus serviceBus, IInfrastructureEnvironment environment)
        {
            Container = shop.System.AddContainer("Shop Frontend",
                "Allows the user to browse and order products",
                "ASP.NET Core MVC Web Application");

            Infrastructure = new AppService
            {
                Name = $"aac-sample-shop-{environment.Name}",
                EnvironmentInvariantName = "aac-sample-shop"
            };

            Uses(api).Over("JSON").Over<Https>().InOrderTo("Loads products and orders");

            Uses(serviceBus).Over(serviceBus.Infrastructure.Queue("order")).InOrderTo("Notify about orders");
        }
    }
}