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

            template.Resources.Add(PostProcess(new JObject
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
                    ["outputs"] = Outputs(streamAnalytics),
                    ["transformation"] = new JObject
                    {
                        ["name"] = "Transformation",
                        ["properties"] = new JObject
                        {
                            ["query"] = streamAnalytics.TransformationQuery,
                            ["streamingUnits"] = 1
                        }
                    }
                },
                ["dependsOn"] = new JArray(streamAnalytics.Inputs.Select(i => i.Source).OfType<IHaveResourceId>().Select(i => i.ResourceIdReference).Cast<object>().ToArray())
            }));
        }

        private static JArray Inputs(StreamAnalytics streamAnalytics)
        {
            var inputs = new JArray();

            foreach (var input in streamAnalytics.Inputs)
            {
                inputs.Add(new JObject
                {
                    ["type"] = "Microsoft.StreamAnalytics/streamingjobs/inputs",
                    ["name"] = input.Name,
                    ["apiVersion"] = "2016-03-01",
                    ["properties"] = new JObject
                    {
                        ["type"] = input.Type,
                        ["datasource"] = InputDataSource(input),
                        ["serialization"] = new JObject
                        {
                            ["type"] = "Json",
                            ["properties"] = new JObject
                            {
                                ["encoding"] = "UTF8"
                            }
                        }
                    },
                    ["dependsOn"] = new JArray(streamAnalytics.ResourceIdReference)
                });


            }

            return inputs;
        }

        private static JArray Outputs(StreamAnalytics streamAnalytics)
        {
            var outputs = new JArray();

            foreach (var output in streamAnalytics.Outputs)
            {
                outputs.Add(new JObject
                {
                    ["type"] = "Microsoft.StreamAnalytics/streamingjobs/outputs",
                    ["name"] = output.Name,
                    ["apiVersion"] = "2016-03-01",
                    ["properties"] = new JObject
                    {
                        ["datasource"] = OutputDataSource(output)
                    },
                    ["dependsOn"] = new JArray(streamAnalytics.ResourceIdReference)
                });
            }

            return outputs;
        }

        private static JObject InputDataSource(StreamAnalyticsInput input)
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
                        ["sharedAccessPolicyKey"] = iotHub.IotHub.OwnerKey.Value.ToString(),
                        ["endpoint"] = "messages/events",
                        ["consumerGroupName"] = "$Default"
                    }
                };
            }

            if (input is EventHubInput eventHub)
            {
                return new JObject
                {
                    ["type"] = "Microsoft.ServiceBus/EventHub",
                    ["properties"] = new JObject
                    {
                        ["serviceBusNamespace"] = eventHub.EventHub.Namespace.Name,
                        ["sharedAccessPolicyName"] = eventHub.EventHub.Namespace.RootManageSharedAccessPolicy.Name,
                        ["sharedAccessPolicyKey"] = eventHub.EventHub.Namespace.RootManageSharedAccessPolicy.Value.ToString(),
                        ["eventHubName"] = eventHub.EventHub.Name,
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
                            ["accountName"] = blobStorage.StorageAccount.Name,
                            ["accountKey"] = blobStorage.StorageAccount.AccountKey
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

        private static JObject OutputDataSource(StreamAnalyticsOutput output)
        {
            if (output is TableOutput tableStorage)
            {
                return new JObject
                {
                    ["type"] = "Microsoft.Storage/Table",
                    ["properties"] = new JObject
                    {
                        ["accountName"] = tableStorage.StorageAccount.Name,
                        ["accountKey"] = tableStorage.StorageAccount.AccountKey,
                        ["table"] = tableStorage.Table,
                        ["partitionKey"] = tableStorage.PartitionKey,
                        ["rowKey"] = tableStorage.RowKey,
                        ["batchSize"] = 100
                    }
                };
            }

            throw new InvalidOperationException("Unknown output to StreamAnalytics: " + output.GetType());
        }
    }
}