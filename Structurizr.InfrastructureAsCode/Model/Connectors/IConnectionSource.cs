using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Model.Connectors
{
    public interface IConnectionSource
    {
        string EnvironmentInvariantName { get; set; }
        IEnumerable<KeyValuePair<string, ConfigurationValue>> GetConnectionInformation();
    }
}