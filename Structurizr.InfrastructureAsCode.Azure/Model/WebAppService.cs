using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class WebAppService : AppService, IHttpsConnectionSource
    {
        public string EnvironmentInvariantName { get; set; }

        IEnumerable<KeyValuePair<string, IConfigurationValue>> IHttpsConnectionSource.ConnectionInformation()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }
            yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-url", Url);
        }
    }
}
