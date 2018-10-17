using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class IoTHub : ContainerInfrastructure, IHaveResourceId
    {
        public IoTHub()
        {
            ConsumerGroups = new List<string>();
        }

        public string EnvironmentInvariantName { get; set; }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Devices/iotHubs', '{Name}')";

        public List<string> ConsumerGroups { get; }

        public IoTHubKey OwnerKey => new IoTHubKey(this, "iothubowner");
        public IoTHubConnectionString OwnerConnectionString => new IoTHubConnectionString(this, OwnerKey);

        public string ApiVersion = "2016-02-03";

        public string Url => Name + ".azure-devices.net";

    }

    public class IoTHubKey : DependentConfigurationValue<IoTHub>, IHaveResourceId
    {
        private readonly IoTHub _hub;
        public string Name { get; }

        public IoTHubKey(IoTHub hub, string name) : base(hub)
        {
            _hub = hub;
            Name = name;
        }
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";

        public string ResourceIdReferenceContent =>
            $"resourceId('Microsoft.Devices/iotHubs/Iothubkeys', '{_hub.Name}', '{Name}')";

        public override object Value => $"[listkeys({ResourceIdReferenceContent}, '{_hub.ApiVersion}').primaryKey]";
        public override bool ShouldBeStoredSecure => true;
    }

    public class IoTHubConnectionString : DependentConfigurationValue<IoTHub>
    {
        private readonly IoTHubKey _key;

        public IoTHubConnectionString(IoTHub ioTHub, IoTHubKey key): base(ioTHub)
        {
            _key = key;
        }

        public override object Value =>
            $"[concat('HostName=', reference({DependsOn.ResourceIdReferenceContent}).hostName, ';SharedAccessKeyName=', '{_key.Name}', ';SharedAccessKey=', listkeys({_key.ResourceIdReferenceContent}, '{DependsOn.ApiVersion}').primaryKey)]";
        public override bool ShouldBeStoredSecure => true;
    }

    public class IoTHubSDK : ContainerConnector<IoTHub>
    {
        public override string Technology => "IoT Hub SDK";

        protected override IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation(IoTHub source)
        {
            if (string.IsNullOrWhiteSpace(source.EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }

            yield return new KeyValuePair<string, IConfigurationValue>(source.EnvironmentInvariantName + "-connection", source.OwnerConnectionString);
        }
    }
}