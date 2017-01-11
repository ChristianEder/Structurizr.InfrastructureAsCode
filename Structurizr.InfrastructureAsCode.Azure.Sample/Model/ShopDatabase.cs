using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopDatabase : Container<NoSqlDocumentDatabase>
    {
        public ShopDatabase() : this(null)
        {

        }

        public ShopDatabase(IInfrastructureEnvironment environment)
        {
            Name = "Shop database";
            Description = "Stores product, customer and order data";
            Technology = "DocumentDB";

            if (environment != null)
            {
                Infrastructure = new NoSqlDocumentDatabase { Name = $"shop-db-{environment.Name}" };
            }
        }
    }
}