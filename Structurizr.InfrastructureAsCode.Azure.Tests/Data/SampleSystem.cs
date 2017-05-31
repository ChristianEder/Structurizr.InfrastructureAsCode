using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Tests.Data
{
    public class SampleSystem : SoftwareSystemWithInfrastructure
    {
        public SampleSystem(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem("Sample", "");
            Customer = workspace.Model.AddPerson(Location.External, "Customer", "Does stuff");

            WebApplication = new SampleWebApplication(this);
            Customer.Uses(WebApplication.Container, "does stuff");

            Customer.Uses(System, "does stuff");
        }

        public Person Customer { get; set; }

        public SampleWebApplication WebApplication { get; private set; }
    }

    public class SampleWebApplication : ContainerWithInfrastructure<AppService>
    {
        public SampleWebApplication(SampleSystem system)
        {
            Container = system.System.AddContainer("Sample", "Sample", "ASP.NET");
            Infrastructure = new AppService
            {
                Name = "sample"
            };
        }
    }
}
