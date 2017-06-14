using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public class Http : ContainerConnector
    {
        public override string Technology => "HTTP";
    }

    public class Https : Http
    {
        public override string Technology => "HTTPS";
    }
}
