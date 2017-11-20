using System.Linq;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public abstract class AppService : ContainerInfrastructure, IHaveHiddenLink, IConfigurable
    {
        protected AppService()
        {
            Settings = new Configuration<AppServiceSetting>();
            ConnectionStrings = new Configuration<AppServiceConnectionString>();
        }

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
}