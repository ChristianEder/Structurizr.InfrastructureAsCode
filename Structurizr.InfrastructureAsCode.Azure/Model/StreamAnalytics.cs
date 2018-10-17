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

        private readonly List<StreamAnalyticsOutput> _outputs = new List<StreamAnalyticsOutput>();
        public IEnumerable<StreamAnalyticsOutput> Outputs => _outputs;
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.StreamAnalytics/streamingjobs', '{Name}')";

        public string TransformationQuery { get; set; } = "SELECT\r\n    *\r\nINTO\r\n    [YourOutputAlias]\r\nFROM\r\n    [YourInputAlias]";

        public IotHubInput IotHubInput(string name, IoTHub iotHub)
        {
            return GetOrAddInput(iotHub, () => new IotHubInput(name, iotHub));
        }
        public EventHubInput EventHubInput(string name, EventHub eventHub)
        {
            return GetOrAddInput(eventHub, () => new EventHubInput(name, eventHub));
        }

        public BlobStorageInput BlobStorageInput(string name, StorageAccount storageAccount, string container)
        {
            return GetOrAddInput(storageAccount, () => new BlobStorageInput(name, storageAccount, container));
        }

        public TableOutput TableStorageOutput(string name, StorageAccount storageAccount, string table, string partitionKey, string rowKey)
        {
            return GetOrAddOutput(storageAccount, () => new TableOutput(name, storageAccount, table, partitionKey, rowKey));
        }

        private TInput GetOrAddInput<TInput>(ContainerInfrastructure source, Func<TInput> create)
            where TInput : StreamAnalyticsInput
        {
            var input = _inputs.OfType<TInput>().FirstOrDefault(i => i.Source == source);
            if (input == null)
            {
                input = create();
                _inputs.Add(input);
            }

            return input;
        }
        private TOutput GetOrAddOutput<TOutput>(ContainerInfrastructure target, Func<TOutput> create)
            where TOutput : StreamAnalyticsOutput
        {
            var output = _outputs.OfType<TOutput>().FirstOrDefault(i => i.Target == target);
            if (output == null)
            {
                output = create();
                _outputs.Add(output);
            }

            return output;
        }

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

    public class EventHubInput : StreamAnalyticsInput
    {
        public EventHub EventHub { get; }

        public EventHubInput(string name, EventHub eventHub) : base(name, eventHub)
        {
            EventHub = eventHub;
        }

        public override string Technology => "Stream Analytics Event Hub input";

        public override string Type => "Stream";
    }

    public abstract class StreamAnalyticsOutput : IContainerConnector
    {
        protected StreamAnalyticsOutput(string name, ContainerInfrastructure target)
        {
            Name = name;
            Target = target;
        }

        public void Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer) where TUsing : ContainerInfrastructure where TUsed : ContainerInfrastructure
        {
        }

        public ContainerInfrastructure Target { get; }

        public string Name { get; }

        public abstract string Technology { get; }
    }

    public class TableOutput : StreamAnalyticsOutput
    {
        public StorageAccount StorageAccount { get; }
        public string Table { get; }
        public string PartitionKey { get; }
        public string RowKey { get; }

        public TableOutput(string name, StorageAccount storageAccount, string table, string partitionKey, string rowKey) : base(name, storageAccount)
        {
            StorageAccount = storageAccount;
            Table = table;
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public override string Technology => "Stream Analytics Table Storage output";
    }
}