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
        void IConfigurable.Configure(string name, IConfigurationValue value, bool secure)
        {
            if (secure)
            {
                ConnectionStrings.Add(new AppServiceConnectionString
                {
                    Name = name,
                    Type = "Custom",
                    Value = value
                });
            }
            else
            {
               Settings.Add(new AppServiceSetting(name, value));
            }
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

    public class AppServiceConnectionString : ConfigurationElement
    {
        public AppServiceConnectionString()
        {

        }

        public AppServiceConnectionString(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = new FixedConfigurationValue<string>(value);
        }

        public string Type { get; set; }
    }
}