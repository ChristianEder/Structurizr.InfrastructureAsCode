using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class StreamAnalyticsRenderer : AzureResourceRenderer<StreamAnalytics>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<StreamAnalytics> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var streamAnalytics = elementWithInfrastructure.Infrastructure;

            template.Resources.Add(new JObject
            {
                ["type"] = "Microsoft.StreamAnalytics/streamingjobs",
                ["name"] = streamAnalytics.Name,
                ["apiVersion"] = "2017-04-01-preview",
                ["location"] = location,
                ["properties"] = new JObject
                {
                    ["outputErrorPolicy"] = "stop",
                    ["eventsOutOfOrderPolicy"] = "adjust",
                    ["eventsOutOfOrderMaxDelayInSeconds"] = 0,
                    ["eventsLateArrivalMaxDelayInSeconds"] = 5,
                    ["dataLocale"] = "en-US",
                    ["jobType"] = "Cloud",
                    ["sku"] = new JObject
                    {
                        ["name"] = "standard"
                    },
                    ["inputs"] = Inputs(streamAnalytics),
                    ["outputs"] = new JArray(),
                    ["transformation"] = new JObject
                    {
                        ["name"] = "Transformation",
                        ["properties"] = new JObject
                        {
                            ["query"] = "SELECT\r\n    *\r\nINTO\r\n    [YourOutputAlias]\r\nFROM\r\n    [YourInputAlias]",
                            ["streamingUnits"] = 1
                        }
                    }
                }
                ["dependsOn"] = new JArray(streamAnalytics.Inputs.Select(i => i.Source).OfType<IHaveResourceId>().Select(i => i.ResourceIdReferenceContent).Cast<object>().ToArray())
            });
        }

        private static JArray Inputs(StreamAnalytics streamAnalytics)
        {
            var inputs = new JArray();

            foreach (var input in streamAnalytics.Inputs)
            {
                inputs.Add(new JObject
                {
                    ["type"] = "Microsoft.StreamAnalytics/streamingjobs/inputs",
                    ["name"] = streamAnalytics.Name + "/" + input.Name,
                    ["apiVersion"] = "2016-03-01",
                    ["properties"] = new JObject
                    {
                        ["type"] = input.Type,
                        ["datasource"] = DataSource(input),
                        ["serialization"] = new JObject
                        {
                            ["type"] ="Json",
                            ["properties"]= new JObject
                            {
                                ["encoding"] = "UTF8"
                            }
                        }
                    }
                    ["dependsOn"]= new JArray(streamAnalytics.ResourceIdReference)
                });


            }

            return inputs;
        }

        private static JObject DataSource(StreamAnalyticsInput input)
        {
            if (input is IotHubInput iotHub)
            {
                return new JObject
                {
                    ["type"] = "Microsoft.Devices/IotHubs",
                    ["properties"] = new JObject
                    {
                        ["iotHubNamespace"] = iotHub.IotHub.Name,
                        ["sharedAccessPolicyName"] = "iothubowner",
                        ["endpoint"] = "messages/events",
                        ["consumerGroupName"] = "$Default"
                    }
                };
            }

            if (input is BlobStorageInput blobStorage)
            {
                return new JObject
                {
                    ["type"] = "Microsoft.Storage/Blob",
                    ["properties"] = new JObject
                    {
                        ["storageAccounts"] = new JArray(new JObject
                        {
                            ["accountName"] = blobStorage.StorageAccount.Name
                        }),
                        ["container"] = blobStorage.Container,
                        ["pathPattern"] = blobStorage.PathPattern,
                        ["dateFormat"] = blobStorage.DateFormat,
                        ["timeFormat"] = blobStorage.TimeFormat
                    }
                };
            }

            throw new InvalidOperationException("Unknown input to StreamAnalytics: " + input.GetType());
        }
    }
}