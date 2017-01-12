namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class AppService : ContainerInfrastructure
    {
        public AppService()
        {
            Settings = new ContainerInfrastructureConfiguration<AppServiceSetting>();
            ConnectionStrings = new ContainerInfrastructureConfiguration<AppServiceConnectionString>();
        }
        public ContainerInfrastructureConfiguration<AppServiceSetting> Settings { get; set; }
        public ContainerInfrastructureConfiguration<AppServiceConnectionString> ConnectionStrings { get; set; }
    }

    public class AppServiceSetting : ContainerInfrastructureConfigurationElement
    {
        public AppServiceSetting()
        {
        }

        public AppServiceSetting(string name, string value)
        {
            Name = name;
            Value = new ContainerInfrastructureConfigurationElementValue<string>(value);
        }

        public string Name { get; set; }
    }

    public class AppServiceConnectionString : ContainerInfrastructureConfigurationElement
    {
        public AppServiceConnectionString()
        {
            
        }

        public AppServiceConnectionString(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = new ContainerInfrastructureConfigurationElementValue<string>(value);
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}
