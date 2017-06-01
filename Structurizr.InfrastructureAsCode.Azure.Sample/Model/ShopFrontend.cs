using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopFrontend : ContainerWithInfrastructure<AppService>
    {
        public ShopFrontend(Shop shop, ShopDatabase database, IInfrastructureEnvironment environment)
        {
            Container = shop.System.AddContainer("Shop Frontend",
                "Allows the user to browse and order products",
                "ASP.NET Core MVC Web Application");

            Container.Uses(database.Container, "Stores and loads products and orders");

            Infrastructure = new AppService { Name = $"aac-sample-shop-{environment.Name}" };

            Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
            {
                Name = "shop-db-uri",
                Type = "Custom",
                Value = database.Infrastructure.Uri
            });
        }
    }
}