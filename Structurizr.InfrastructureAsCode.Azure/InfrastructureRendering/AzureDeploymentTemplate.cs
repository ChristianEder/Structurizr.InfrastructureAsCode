using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class AzureDeploymentTemplate
    {
        private readonly string _contentVersion;

        public AzureDeploymentTemplate(string contentVersion)
        {
            _contentVersion = contentVersion;
            Variables = new Dictionary<string, string>();
            Resources = new List<JObject>();
            Parameters = new JObject();
            ParameterValues = new JObject();
        }

        public JObject Parameters { get; }
        public JObject ParameterValues { get; }
        public Dictionary<string, string> Variables { get; }
        public List<JObject> Resources { get; }

        public override string ToString()
        {
            var template = new JObject
            {
                ["$schema"] = "http://schema.management.azure.com/schemas/2014-04-01-preview/deploymentTemplate.json#",
                ["contentVersion"] = _contentVersion,
                ["parameters"] = Parameters,
                ["resources"] = new JArray(Resources)
            };

            foreach (var variable in Variables)
            {
                template["variables"][variable.Key] = variable.Value;
            }

            return template.ToString();
        }
    }
}
