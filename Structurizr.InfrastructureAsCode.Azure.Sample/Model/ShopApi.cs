using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopApi : Container<AppService>
    {
        private readonly ShopDatabase _database;

        public ShopApi(ShopDatabase database) : this(null, database)
        {
        }

        public ShopApi(IInfrastructureEnvironment environment, ShopDatabase database)
        {
            _database = database;
            Name = "Shop API";
            Description = "Provides a service layer to browse and order products";
            Technology = "ASP.NET Core MVC Web Application";

            if (environment != null)
            {
                Infrastructure = new AppService { Name = $"aac-sample-shop-api-{environment.Name}" };

                Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
                {
                    Name = "shop-db-uri",
                    Type = "Custom",
                    Value = database.Infrastructure.Uri
                });
            }
        }

        public override void InitializeUsings()
        {
            base.InitializeUsings();
            Uses(_database, "Stores and loads products and orders");
        }
    }
}