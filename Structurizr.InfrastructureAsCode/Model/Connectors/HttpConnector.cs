using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public class Http : ContainerConnector<IHttpConnectionSource>
    {
        public override string Technology => "HTTP";
        protected override IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation(IHttpConnectionSource source)
        {
            return source.ConnectionInformation();
        }
    }

    public class Https : ContainerConnector<IHttpsConnectionSource>
    {
        public override string Technology => "HTTPS";
        protected override IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation(IHttpsConnectionSource source)
        {
            return source.ConnectionInformation();
        }
    }

    public interface IHttpConnectionSource
    {
        IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation();
    }

    public interface IHttpsConnectionSource
    {
        IEnumerable<KeyValuePair<string, IConfigurationValue>> ConnectionInformation();
    }
}
