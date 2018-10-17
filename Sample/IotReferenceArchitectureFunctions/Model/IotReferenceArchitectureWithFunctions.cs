using Structurizr;
using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchitectureWithFunctions : SoftwareSystemWithInfrastructure
    {
        public CloudGateway CloudGateway { get; }
        public DeviceProvisioning DeviceProvisioning { get; }
        public SecretStorage SecretStorage { get; }
        public TelemetryStorage TelemetryStorage { get; }
        public Ingress Ingress { get; }
        public ApplicationInsights ApplicationInsights { get; }
        public MasterDataStorage MasterDataStorage { get; }
        public SanitizedMessages SanitizedMessages { get; }
        public StreamAnalytics StreamAnalytics { get; }


        public IotReferenceArchitectureWithFunctions(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem(
                Location.Internal,
                "Monkey Factory",
                "Azure cloud based backend for processing data created during production of monkeys");

            SecretStorage = new SecretStorage(this, environment);
            CloudGateway = new CloudGateway(this, environment);
            DeviceProvisioning = new DeviceProvisioning(this, CloudGateway, environment);
            TelemetryStorage = new TelemetryStorage(this, environment);
            ApplicationInsights = new ApplicationInsights(this, environment);
            MasterDataStorage = new MasterDataStorage(this, new SqlServer(environment), environment);
            SanitizedMessages = new SanitizedMessages(this, new EventHubNamespace(environment), environment);
            Ingress = new Ingress(this, CloudGateway, TelemetryStorage, MasterDataStorage, SanitizedMessages, ApplicationInsights, environment);
            StreamAnalytics = new StreamAnalytics(this, SanitizedMessages, TelemetryStorage, environment);

            Device = workspace.Model.AddSoftwareSystem(Location.External, "Device", "Sends data into the cloud");
            Device.Uses(CloudGateway, "Send production telemetry data and failure events", "MQTT");
            Device.Uses(DeviceProvisioning, "Provision device", "HTTPS");
        }

        public SoftwareSystem Device { get; }
    }
}