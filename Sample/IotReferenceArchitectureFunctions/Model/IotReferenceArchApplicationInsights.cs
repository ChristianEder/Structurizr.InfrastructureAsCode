using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchApplicationInsights : ContainerWithInfrastructure<ApplicationInsights>
    {

        public IotReferenceArchApplicationInsights(IotReferenceArchModel iotReferenceArchModel, 
            IInfrastructureEnvironment environment)
        {

            Container = iotReferenceArchModel.System.AddContainer(
                name: "AI",
                description: "Logging",
                technology: "Azure Application Insights");

            Infrastructure = new ApplicationInsights()
            {
                Name = "ref-ai-" + environment.Name
            };
        }


    }
}
