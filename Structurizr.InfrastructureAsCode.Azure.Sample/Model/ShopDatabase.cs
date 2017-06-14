using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopDatabase : ContainerWithInfrastructure<NoSqlDocumentDatabase>
    {
        public ShopDatabase(Shop shop, IInfrastructureEnvironment environment)
        {
            Container = shop.System.AddContainer(
                "Shop database",
                "Stores product, customer and order data",
                "DocumentDB");

            Infrastructure = new NoSqlDocumentDatabase
            {
                Name = $"aac-sample-shop-db-{environment.Name}",
                EnvironmentInvariantName = "aac-sample-shop-db"
            };
        }
    }
}