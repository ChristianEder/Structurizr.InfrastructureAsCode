using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class IoTHub : ContainerInfrastructure, IHaveResourceId
    {
        public IoTHub()
        {
            ConsumerGroups = new List<string>();
        }
        
        public string ResourceIdReference => $"[resourceId('Microsoft.Devices/Iothubs', '{Name}')]";

        public List<string> ConsumerGroups { get; }

        public IoTHubKey OwnerKey => new IoTHubKey(this, "iothubowner");
        public IoTHubConnectionString OwnerConnectionString => new IoTHubConnectionString(this, OwnerKey);

        public string ApiVersion = "2016-02-03";
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
        public string ResourceIdReference =>
            $"[resourceId('Microsoft.Devices/Iothubs/Iothubkeys', '{_hub.Name}', '{Name}')]";

        public override object Value => $"[listkeys('{ResourceIdReference}', {_hub.ApiVersion}).primaryKey]";
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
            $"[concat('HostName=', reference('{DependsOn.ResourceIdReference}').hostName, ';SharedAccessKeyName=', '{_key.Name}', ';SharedAccessKey=', listkeys('{_key.ResourceIdReference}', '{DependsOn.ApiVersion}').primaryKey)]";
        public override bool ShouldBeStoredSecure => true;
    }
}