using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Model;

namespace Structurizr.InfrastructureAsCode.Azure.Sample.Model
{
    public class MonkeyFactory : SoftwareSystemWithInfrastructure
    {
        public MonkeyKeyVault KeyVault { get; }
        public MonkeyHub Hub { get; }
        public MonkeyMessageProcessor MessageProcessor { get; }
        public MonkeyUI UI { get; }
        public MonkeyCrmConnector CrmConnector { get; }
        public MonkeyEventStore EventStore { get; }
        public Person ProductionShiftLead { get; }
        public Person TechnicalSupportUser { get; }
        public SoftwareSystem MonkeyProductionLine { get; }
        public SoftwareSystem Crm { get; }

        public MonkeyDeviceProvisioningService DPS { get; }

        public MonkeyFactory(Workspace workspace, IInfrastructureEnvironment environment)
        {
            System = workspace.Model.AddSoftwareSystem(
                Location.Internal, 
                "Monkey Factory",
                "Azure cloud based backend for processing data created during production of monkeys");

            KeyVault = new MonkeyKeyVault(this, environment);
            Hub = new MonkeyHub(this, environment);
            CrmConnector = new MonkeyCrmConnector(this, environment);
            EventStore = new MonkeyEventStore(this, environment);
            UI = new MonkeyUI(this, EventStore, environment);
            MessageProcessor = new MonkeyMessageProcessor(this, Hub, CrmConnector, EventStore, environment);
            DPS = new MonkeyDeviceProvisioningService(this, Hub, environment);

            TechnicalSupportUser = workspace.Model.AddPerson("Technical support user", "Responds to incidents during monkey production");
            TechnicalSupportUser.Uses(UI, "Gather information about system failures");

            ProductionShiftLead = workspace.Model.AddPerson("Production Shift leader", "Monitors monkey production");
            ProductionShiftLead.Uses(UI, "Monitor load on production systems");

            MonkeyProductionLine = workspace.Model.AddSoftwareSystem(Location.External, "Production Line", "Produces the actual monkeys");
            MonkeyProductionLine.Uses(Hub, "Send production telemetry data and failure events", "MQTT");
            MonkeyProductionLine.Uses(DPS, "Provision device that produces monkeys", "HTTPS");

            Crm = workspace.Model.AddSoftwareSystem(Location.External, "CRM", "");
            Crm.Uses(CrmConnector, "Process failure events in order to create support tickets", "AMQP");

            StoreSecretsInKeyVault();
        }

        private void StoreSecretsInKeyVault()
        {
            UI.Infrastructure.UseStore(KeyVault.Infrastructure);
            MessageProcessor.Infrastructure.UseStore(KeyVault.Infrastructure);
        }
    }
}
