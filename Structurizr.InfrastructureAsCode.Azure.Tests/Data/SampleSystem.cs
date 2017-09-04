using System;
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

            Api = new SampleApi(this);
            WebApplication = new SampleWebApplication(this, Api);
            Customer.Uses(WebApplication.Container, "does stuff");

            Customer.Uses(System, "does stuff");
        }

        public Person Customer { get; set; }

        public SampleWebApplication WebApplication { get; }
        public SampleApi Api { get; }
    }

    public class SampleWebApplication : ContainerWithInfrastructure<WebAppService>
    {
        public SampleWebApplication(SampleSystem system, SampleApi api)
        {
            Container = system.System.AddContainer("Sample", "Sample", "ASP.NET");
            Container.Uses(api.Container, "loads data from");

            Infrastructure = new WebAppService
            {
                Name = "sample"
            };

            Infrastructure.Settings.Add(new AppServiceSetting
            {
                Name = "apiUrl",
                Value = api.Infrastructure.Url
            });
        }
    }
    public class SampleApi : ContainerWithInfrastructure<WebAppService>
    {
        public SampleApi(SampleSystem system)
        {
            Container = system.System.AddContainer("Sample API", "Sample API", "ASP.NET");
            Infrastructure = new WebAppService
            {
                Name = "sampleapi"
            };
        }
    }
}
