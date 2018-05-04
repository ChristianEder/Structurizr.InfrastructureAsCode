using Structurizr;
using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model;

namespace IotReferenceArchitectureFunctions.Model
{
    public class IotReferenceArchModel : SoftwareSystemWithInfrastructure
    {
        public IotReferenceArchHub Hub { get; }

        public IotReferenceArchDPS DPS { get; }

        public IotReferenceArchKeyVault KeyVault { get; }

        public IotReferenceArchTelemetryStorage TelemetryStorage { get; }

        public IotReferenceArchIngress Ingress { get; }
        public IotReferenceArchApplicationInsights AppInsights { get; }

        public IotReferenceArchModel(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem(
                Location.Internal,
                "Monkey Factory",
                "Azure cloud based backend for processing data created during production of monkeys");

            KeyVault = new IotReferenceArchKeyVault(this, environment);
            Hub = new IotReferenceArchHub(this, environment);
            DPS = new IotReferenceArchDPS(this, Hub, environment);
            TelemetryStorage = new IotReferenceArchTelemetryStorage(this, environment);
            AppInsights = new IotReferenceArchApplicationInsights(this, environment);
            Ingress = new IotReferenceArchIngress(this, Hub, TelemetryStorage, AppInsights, environment);

            Device = workspace.Model.AddSoftwareSystem(Location.External,"Device", "Sends data into cloud");
            Device.Uses(Hub, "Send production telemetry data and failure events", "MQTT");
            Device.Uses(DPS, "Provision device", "HTTPS");
        }

        public SoftwareSystem Device { get; }
    }
}