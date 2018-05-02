using Microsoft.Azure.Management.Compute.Fluent.Models;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyEventStore : ContainerWithInfrastructure<StorageAccount>
    {
        public MonkeyEventStore(MonkeyFactory monkeyFactory, IInfrastructureEnvironment environment)
        {
            Container = monkeyFactory.System.AddContainer(
                name: "Monkey Event Store",
                description: "Persistently stores all received monkey production telemetry and event data",
                technology: "Azure Blob Storage");

            Infrastructure = new StorageAccount
            {
                Kind = StorageAccountKind.StorageV2,
                Name = "monkeyevents" + environment.Name,
                EnvironmentInvariantName = "monkeyevents"
            };
        }
    }
}