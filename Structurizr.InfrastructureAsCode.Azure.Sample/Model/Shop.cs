using System;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class Shop : SoftwareSystemWithInfrastructure
    {

        public Shop(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem("Shop", "");
            Customer = workspace.Model.AddPerson(Location.External, "Customer", "Buys stuff in our shop");

            ServiceBus = new ShopServiceBus(this, environment);
            Database = new ShopDatabase(this, environment);
            Api = new ShopApi(this, Database, ServiceBus, environment);
            Frontend = new ShopFrontend(this, Api, ServiceBus, environment);

            Customer.Uses(Frontend.Container, "buys stuff");

            Customer.Uses(System, "buys stuff");
        }

        public ShopServiceBus ServiceBus { get; }
        public ShopFrontend Frontend { get; }
        public ShopApi Api { get; }
        public ShopDatabase Database { get; }
        public Person Customer { get; }
    }
}
