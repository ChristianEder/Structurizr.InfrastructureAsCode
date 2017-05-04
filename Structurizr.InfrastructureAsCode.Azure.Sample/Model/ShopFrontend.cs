using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopFrontend : Container<AppService>
    {
        private readonly ShopApi _api;

        public ShopFrontend(ShopApi api) : this(null, api)
        {
        }

        public ShopFrontend(IInfrastructureEnvironment environment, ShopApi api)
        {
            _api = api;
            Name = "Shop Frontend";
            Description = "Allows the user to browse and order products";
            Technology = "ASP.NET Core MVC Web Application";

            if (environment != null)
            {
                Infrastructure = new AppService { Name = $"aac-sample-shop-{environment.Name}" };

                Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
                {
                    Name = "shop-api-uri",
                    Type = "Custom",
                    Value = _api.Infrastructure.Url
                });
            }
        }

        public override void InitializeUsings()
        {
            base.InitializeUsings();
            Uses(_api, "Stores and loads products and orders");
        }
    }
}