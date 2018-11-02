using System;
using System.Linq;
using Microsoft.Azure.Management.AppService.Fluent;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class ServiceBusRenderer : AzureResourceRenderer<ServiceBus>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<ServiceBus> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var serviceBus = elementWithInfrastructure.Infrastructure;
            AddNamespace(template, serviceBus, location);

            foreach (var queue in serviceBus.Queues)
            {
                template.Resources.Add(PostProcess(new JObject
                {
                    ["name"] = $"{serviceBus.Name}/{queue}",
                    ["type"] = "Microsoft.ServiceBus/namespaces/queues",
                    ["apiVersion"] = "2015-08-01",
                    ["location"] = location,
                    ["properties"] = new JObject
                    {
                        ["defaultMessageTimeToLive"] = "14.00:00:00",
                        ["maxSizeInMegabytes"] = "1024",
                        ["deadLetteringOnMessageExpiration"] = false,
                        ["requiresDuplicateDetection"] = false,
                        ["requiresSession"] = false,
                        ["enablePartitioning"] = true,
                    },
                    ["dependsOn"] = new JArray
                    {
                        serviceBus.ResourceIdReference
                    }
                }));
            }

            foreach (var topic in serviceBus.Topics)
            {
                template.Resources.Add(PostProcess(new JObject
                {
                    ["name"] = $"{serviceBus.Name}/{topic}",
                    ["type"] = "Microsoft.ServiceBus/namespaces/topics",
                    ["apiVersion"] = "2015-08-01",
                    ["location"] = location,
                    ["properties"] = new JObject
                    {
                        ["defaultMessageTimeToLive"] = "14.00:00:00",
                        ["maxSizeInMegabytes"] = "1024",
                        ["requiresDuplicateDetection"] = false,
                        ["enablePartitioning"] = true,
                    },
                    ["dependsOn"] = new JArray
                    {
                        $"[resourceId('Microsoft.ServiceBus/namespaces', '{serviceBus.Name}')]"
                    }
                }));
            }
        }

        private void AddNamespace(AzureDeploymentTemplate template,
            ServiceBus serviceBus, string location)
        {
            var sku = "1";

            if (serviceBus.Topics.Any())
            {
                sku = "2";
            }

            template.Resources.Add(PostProcess(new JObject
            {
                ["type"] = "Microsoft.ServiceBus/namespaces",
                ["name"] = serviceBus.Name,
                ["apiVersion"] = "2014-09-01",
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["MessagingSku"] = sku,
                    ["MessagingSKUPlan"] = new JObject
                    {
                        ["MessagingUnits"] = "1",
                        ["SKU"] = sku
                    }
                }
            }));
        }
    }
}