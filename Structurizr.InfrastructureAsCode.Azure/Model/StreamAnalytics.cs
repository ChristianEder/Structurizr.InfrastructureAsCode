using System;
using System.Collections.Generic;
using System.Linq;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class StreamAnalytics : ContainerInfrastructure, IHaveResourceId
    {
        private readonly List<StreamAnalyticsInput> _inputs = new List<StreamAnalyticsInput>();
        public IEnumerable<StreamAnalyticsInput> Inputs => _inputs;

        public IotHubInput Input(string name, IoTHub iotHub)
        {
            return GetOrAddInput(iotHub, () => new IotHubInput(name, iotHub));
        }

        public BlobStorageInput Input(string name, StorageAccount blobStorageAccount, string container)
        {
            return GetOrAddInput(blobStorageAccount, () => new BlobStorageInput(name, blobStorageAccount, container));
        }

        private TInput GetOrAddInput<TInput>(ContainerInfrastructure iotHub, Func<TInput> create)
            where TInput : StreamAnalyticsInput
        {
            var input = _inputs.OfType<TInput>().FirstOrDefault(i => i.Source == iotHub);
            if (input == null)
            {
                input = create();
                _inputs.Add(input);
            }

            return input;
        }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.StreamAnalytics/streamingjobs', '{Name}')";
    }

    public abstract class StreamAnalyticsInput : IContainerConnector
    {
        protected StreamAnalyticsInput(string name, ContainerInfrastructure source)
        {
            Name = name;
            Source = source;
        }

        public void Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer) where TUsing : ContainerInfrastructure where TUsed : ContainerInfrastructure
        {
        }

        public ContainerInfrastructure Source { get; }

        public string Name { get; }

        public abstract string Technology { get; }
        public abstract string Type { get; }
    }

    public class IotHubInput : StreamAnalyticsInput
    {
        public IoTHub IotHub { get; }

        public IotHubInput(string name, IoTHub iotHub) : base(name, iotHub)
        {
            IotHub = iotHub;
        }

        public override string Technology => "Stream Analytics IoT Hub input";

        public override string Type => "Stream";
    }

    public class BlobStorageInput : StreamAnalyticsInput
    {
        public StorageAccount StorageAccount { get; }
        public string PathPattern { get; private set; } = "";
        public string DateFormat { get; private set; } = "yyyy/MM/dd";
        public string TimeFormat { get; private set; } = "HH";
        public string Container { get; }

        public BlobStorageInput(string name, StorageAccount storageAccount, string container) : base(name, storageAccount)
        {
            StorageAccount = storageAccount;
            Container = container;
        }

        public override string Technology => "Stream Analytics Blob Storage input";

        public override string Type => "Stream";

        public BlobStorageInput WithPathPattern(string pattern)
        {
            PathPattern = pattern;
            return this;
        }

        public BlobStorageInput WithDateFormat(string format)
        {
            DateFormat = format;
            return this;
        }

        public BlobStorageInput WithTimeFormat(string format)
        {
            TimeFormat = format;
            return this;
        }
    }
}