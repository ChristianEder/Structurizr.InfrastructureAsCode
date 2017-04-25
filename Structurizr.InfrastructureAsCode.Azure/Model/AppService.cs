namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class AppService : ContainerInfrastructure
    {
        public AppService()
        {
            Settings = new Configuration<AppServiceSetting>();
            ConnectionStrings = new Configuration<AppServiceConnectionString>();
        }
        public Configuration<AppServiceSetting> Settings { get; set; }
        public Configuration<AppServiceConnectionString> ConnectionStrings { get; set; }

        public AppServiceUrl Url => new AppServiceUrl(this);
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
