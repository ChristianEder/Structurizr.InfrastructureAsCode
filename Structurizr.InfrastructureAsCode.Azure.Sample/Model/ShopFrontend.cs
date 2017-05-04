using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class ShopFrontend : Container<AppService>
    {
        private readonly ShopDatabase _database;
        //private readonly ShopKeyVault _keyVault;

        public ShopFrontend(ShopDatabase database) : this(null, database)
        {
        }

        public ShopFrontend(IInfrastructureEnvironment environment, ShopDatabase database)
        {
            _database = database;
            Name = "Shop Frontend";
            Description = "Allows the user to browse and order products";
            Technology = "ASP.NET Core MVC Web Application";

            if (environment != null)
            {
                Infrastructure = new AppService { Name = $"aac-sample-shop-{environment.Name}" };

                Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
                {
                    Name = "shop-db-uri",
                    Type = "Custom",
                    Value = database.Infrastructure.Uri
                });

                //AddKeyVaultAccessConfiguration(keyVault);

                //keyVault.Infrastructure.Secrets.Add(new KeyVaultSecret
                //{
                //    Name = "shop-frontend-db-key",
                //    Value = database.Infrastructure.PrimaryMasterKey
                //});
            }
        }

        public override void InitializeUsings()
        {
            base.InitializeUsings();
            Uses(_database, "Stores and loads products and orders");
            //Uses(_keyVault, "Loads access secrets to the shop database");
        }

        private void AddKeyVaultAccessConfiguration(ShopKeyVault keyVault)
        {
            //Infrastructure.Settings.Add(new AppServiceSetting
            //{
            //    Name = "KeyVaultUrl",
            //    Value = keyVault.Infrastructure.Url
            //});

            //Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
            //{
            //    Name = "KeyVaultAppId",
            //    Type = "Custom",
            //    Value = keyVault.Infrastructure.ActiveDirectoryApplicationIdFor(Infrastructure.Name)
            //});

            //Infrastructure.ConnectionStrings.Add(new AppServiceConnectionString
            //{
            //    Name = "KeyVaultAppSecret",
            //    Type = "Custom",
            //    Value = keyVault.Infrastructure.ActiveDirectoryApplicationSecretFor(Infrastructure.Name)
            //});
        }
    }
}