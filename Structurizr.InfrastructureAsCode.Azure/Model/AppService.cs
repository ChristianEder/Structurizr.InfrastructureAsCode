using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{

    public class AppService : ContainerInfrastructure, IHttpsConnectionSource, IConfigurable
    {
        public AppService()
        {
            Settings = new Configuration<AppServiceSetting>();
            ConnectionStrings = new Configuration<AppServiceConnectionString>();
        }


        public Configuration<AppServiceSetting> Settings { get; set; }
        public Configuration<AppServiceConnectionString> ConnectionStrings { get; set; }

        public AppServiceUrl Url => new AppServiceUrl(this);

        public string EnvironmentInvariantName { get; set; }

        IEnumerable<KeyValuePair<string, ConfigurationValue>> IHttpsConnectionSource.ConnectionInformation()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }
            yield return new KeyValuePair<string, ConfigurationValue>(EnvironmentInvariantName + "-url", Url);
        }

        void IConfigurable.Configure(string name, ConfigurationValue value)
        {
            ConnectionStrings.Add(new AppServiceConnectionString
            {
                Name = name,
                Type = "Custom",
                Value = value
            });
        }
    }

    public class AppServiceSetting : ConfigurationElement
    {
        public AppServiceSetting()
        {
        }

        public AppServiceSetting(string name, string value)
        {
            Name = name;
            Value = new ConfigurationValue<string>(value);
        }

        public string Name { get; set; }
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
            Value = new ConfigurationValue<string>(value);
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class AppServiceUrl : ConfigurationValue
    {
        public AppService AppService { get; }

        public AppServiceUrl(AppService appService)
        {
            AppService = appService;
        }
    }
}
