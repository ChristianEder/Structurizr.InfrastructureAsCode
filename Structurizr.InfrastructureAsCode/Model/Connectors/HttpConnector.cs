using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public class Http : ContainerConnector<IHttpConnectionSource>
    {
        public override string Technology => "HTTP";
        protected override IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation(IHttpConnectionSource source)
        {
            return source.ConnectionInformation();
        }
    }

    public class Https : ContainerConnector<IHttpsConnectionSource>
    {
        public override string Technology => "HTTPS";
        protected override IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation(IHttpsConnectionSource source)
        {
            return source.ConnectionInformation();
        }
    }

    public interface IHttpConnectionSource
    {
        IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation();
    }

    public interface IHttpsConnectionSource
    {
        IEnumerable<KeyValuePair<string, ConfigurationValue>> ConnectionInformation();
    }
}
