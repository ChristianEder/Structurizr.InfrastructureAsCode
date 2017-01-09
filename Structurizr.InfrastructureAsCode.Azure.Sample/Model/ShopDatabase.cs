using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopDatabase : Container
    {
        public ShopDatabase()
        {
            Name = "Shop database";
            Description = "Stores product, customer and order data";
            Technology = "DocumentDB";
            Infrastructure = new NoSqlDocumentDatabase(e => $"shop-db-{e.Name}");
        }
    }
}