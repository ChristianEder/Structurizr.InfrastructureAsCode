using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class JsonDefinedInfrastructureRenderer : AzureResourceRenderer<JsonDefinedInfrastructure>
    {
        protected override void Render(AzureDeploymentTemplate template, ContainerWithInfrastructure<JsonDefinedInfrastructure> container, IAzureInfrastructureEnvironment environment,
            string resourceGroup, string location)
        {
            var resourcesTemplate = JObject.Parse(container.Infrastructure.Template);

            var parameters = resourcesTemplate["parameters"] as IEnumerable<KeyValuePair<string, JToken>>;
            if (parameters != null && parameters.Any())
            {
                throw new InvalidOperationException("Parameters are not supported, please use variables instead. You can set those depending on the target environment.");
            }

            var variables = resourcesTemplate["variables"] as JObject;
            if (variables != null)
            {
                foreach (var v in variables)
                {
                   template.Variables.Add(v.Key, v.Value.ToString());
                }
            }

            var resources = resourcesTemplate["resources"] as JArray;
            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    template.Resources.Add((JObject) resource);
                }
            }
        }
    }
}