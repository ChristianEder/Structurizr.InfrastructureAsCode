using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopFrontend : ContainerWithInfrastructure<AppService>
    {
        public ShopFrontend(Shop shop, ShopApi api, IInfrastructureEnvironment environment)
        {
            Container = shop.System.AddContainer("Shop Frontend",
                "Allows the user to browse and order products",
                "ASP.NET Core MVC Web Application");

            Container.Uses(api.Container, "Stores and loads products and orders");

            Infrastructure = new AppService { Name = $"aac-sample-shop-{environment.Name}" };

            Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
            {
                Name = "shop-api-uri",
                Type = "Custom",
                Value = api.Infrastructure.Url
            });
        }
    }
}