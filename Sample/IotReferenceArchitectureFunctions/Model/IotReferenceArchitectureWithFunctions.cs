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
        public RestApi RestApi { get; }
        public UserInterface UserInterface { get; }



        public IotReferenceArchitectureWithFunctions(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem(
                Location.Internal,
                "Azure IoT system",
                "Azure cloud based backend for processing data created on sensors / devices");

            SecretStorage = new SecretStorage(this, environment);
            CloudGateway = new CloudGateway(this, environment);
            DeviceProvisioning = new DeviceProvisioning(this, CloudGateway, environment);
            TelemetryStorage = new TelemetryStorage(this, environment);
            ApplicationInsights = new ApplicationInsights(this, environment);
            MasterDataStorage = new MasterDataStorage(this, new SqlServer(environment), environment);
            SanitizedMessages = new SanitizedMessages(this, new EventHubNamespace(environment), environment);
            Ingress = new Ingress(this, CloudGateway, TelemetryStorage, MasterDataStorage, SanitizedMessages, ApplicationInsights, environment);
            StreamAnalytics = new StreamAnalytics(this, SanitizedMessages, TelemetryStorage, environment);
            RestApi = new RestApi(this, TelemetryStorage, MasterDataStorage, ApplicationInsights, environment);
            UserInterface = new UserInterface(this, RestApi, environment);

            Device = workspace.Model.AddSoftwareSystem(Location.External, "Device", "Sends data into the cloud");
            Device.Uses(CloudGateway, "Send telemetry data and failure events", "MQTT");
            Device.Uses(DeviceProvisioning, "Provision device", "HTTPS");

            User = workspace.Model.AddPerson("User", "Person using the system to view data and take action on it");
            User.Uses(UserInterface, "View devices and their raw and aggregated data");
        }

        public SoftwareSystem Device { get; }
        public Person User { get; }
    }
}