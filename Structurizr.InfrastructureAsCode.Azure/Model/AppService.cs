using System.Linq;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public abstract class AppService : ContainerInfrastructure, IHaveHiddenLink, IConfigurable, IHaveServiceIdentity, IHaveResourceId
    {
        private ISecureConfigurationStore _store;

        protected AppService()
        {
            Settings = new Configuration<AppServiceSetting>();
            ConnectionStrings = new Configuration<AppServiceConnectionString>();
        }

        public AppServiceUrl Url => new AppServiceUrl(this);

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Web/sites', '{Name}')";

        public string HiddenLink =>
            $"[concat('hidden-link:', resourceGroup().id, '/providers/Microsoft.Web/sites/', '{Name}')]";

        public Configuration<AppServiceSetting> Settings { get; set; }
        public Configuration<AppServiceConnectionString> ConnectionStrings { get; set; }
        void IConfigurable.Configure(string name, IConfigurationValue value)
        {
            if (value.ShouldBeStoredSecure)
            {
                var type = ConnectionStringType.Custom;
                if (value is CosmosDocumentDatabaseAccessKey)
                {
                    type = ConnectionStringType.DocDb;
                }
                if (value is ServiceBusConnectionString)
                {
                    type = ConnectionStringType.ServiceBus;
                }

                ConnectionStrings.Add(new AppServiceConnectionString
                {
                    Name = name,
                    Type = type.ToString(),
                    Value = value
                });
            }
            else
            {
               Settings.Add(new AppServiceSetting(name, value));
            }

            MoveSettingsIntoStore();
        }

        bool IConfigurable.IsConfigurationDependentOn(IHaveInfrastructure other)
        {
            var resource = other.Infrastructure as IHaveResourceId;
            if (resource == null)
            {
                return false;
            }

            return Settings.Concat(ConnectionStrings)
                .Select(s => s.Value)
                .OfType<IDependentConfigurationValue>()
                .Any(v => v.DependsOn == resource);
        }

        public void UseStore(ISecureConfigurationStore store)
        {
            _store = store;
            MoveSettingsIntoStore();
        }

        private void MoveSettingsIntoStore()
        {
            if (_store is null)
            {
                return;
            }

            MoveSettingsIntoStore(Settings);
            MoveSettingsIntoStore(ConnectionStrings);
        }

        private void MoveSettingsIntoStore<T>(Configuration<T> settings) 
            where T : ConfigurationElement
        {
            var secureSettings = settings.Where(s => s.Value.ShouldBeStoredSecure).ToArray();
            if (!secureSettings.Any())
            {
                return;
            }

            ConfigureKeyVaultAccess();
            foreach (var secureSetting in secureSettings)
            {
                settings.Remove(secureSetting);
                _store.Store(secureSetting.Name, secureSetting.Value);
            }
        }

        private void ConfigureKeyVaultAccess()
        {
            var name = $"{_store.EnvironmentInvariantName}-url";
            if (Settings.Any(s => s.Name == name))
            {
                return;
            }
            Settings.Add(new AppServiceSetting(name, _store.Url));

            UseSystemAssignedIdentity = true;
            _store.AllowAccessFrom(this);
        }

        public bool UseSystemAssignedIdentity { get; private set; }

        string IHaveServiceIdentity.Id =>
            $"[reference(concat(resourceId('Microsoft.Web/sites', '{Name}'), '/providers/Microsoft.ManagedIdentity/Identities/default'), '2015-08-31-PREVIEW').principalId]";
    }

    public class AppServiceSetting : ConfigurationElement
    {
        public AppServiceSetting()
        {
        }

        public AppServiceSetting(string name, IConfigurationValue value)
        {
            Name = name;
            Value = value;
        }

        public AppServiceSetting(string name, string value)
        {
            Name = name;
            Value = new FixedConfigurationValue<string>(value);
        }
    }

    public class AppServiceConnectionString : AppServiceSetting
    {
        public AppServiceConnectionString()
        {
        }

        public AppServiceConnectionString(string name, string type, string value) : base(name, value)
        {
            Type = type;
        }

        public string Type { get; set; }
    }

    public class AppServiceUrl : DependentConfigurationValue<AppService>
    {
        public AppService AppService { get; }
        public override bool ShouldBeStoredSecure => false;
        public override object Value => $"[concat('https://', reference({DependsOn.ResourceIdReferenceContent}, '2016-03-01').defaultHostName)]";

        public string DefaultHostName => $"[ reference({DependsOn.ResourceIdReferenceContent}, '2016-03-01').defaultHostName]";

        public AppServiceUrl(AppService appService) : base(appService)
        {
            AppService = appService;
        }
    }
}