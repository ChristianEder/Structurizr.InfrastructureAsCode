using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopApi : ContainerWithInfrastructure<AppService>
    {
        public ShopApi(Shop shop, ShopDatabase database, IInfrastructureEnvironment environment)
        {
            Container = shop.System.AddContainer(
                "Shop API",
                "Provides a service layer to browse and order products",
                "ASP.NET Core MVC Web Application");

            Container.Uses(database.Container, "Stores and loads products and orders");

            Infrastructure = new AppService { Name = $"aac-sample-shop-api-{environment.Name}" };

            Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
            {
                Name = "shop-db-uri",
                Type = "Custom",
                Value = database.Infrastructure.Uri
            });
        }
    }
}