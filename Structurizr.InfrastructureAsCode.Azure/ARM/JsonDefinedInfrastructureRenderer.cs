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
            Merge(template.Parameters, resourcesTemplate);

            if (container.Infrastructure.Parameters != null)
            {
                Merge(template.ParameterValues, JObject.Parse(container.Infrastructure.Parameters));
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

        private static void Merge(JObject target, JObject source)
        {
            var parameters = source["parameters"] as JObject;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    if (target[parameter.Key] != null)
                    {
                        throw new InvalidOperationException("Parameter " + parameter.Key + " already exists.");
                    }

                    target[parameter.Key] = parameter.Value;
                }
            }
        }
    }
}