using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class WebAppService : AppService, IHttpsConnectionSource, IHaveResourceId
    {
        public WebAppServiceUrl Url => new WebAppServiceUrl(this);

        public string EnvironmentInvariantName { get; set; }

        IEnumerable<KeyValuePair<string, IConfigurationValue>> IHttpsConnectionSource.ConnectionInformation()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentInvariantName))
            {
                throw new InvalidOperationException("You have to set the EnvironmentInvariantName in order to use this as a source of connections");
            }
            yield return new KeyValuePair<string, IConfigurationValue>(EnvironmentInvariantName + "-url", Url);
        }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Web/sites', '{Name}')";
    }

   

    public class WebAppServiceUrl : DependentConfigurationValue<WebAppService>
    {
        public WebAppService AppService { get; }
        public override bool ShouldBeStoredSecure => false;
        public override object Value => $"[concat('https://', reference({DependsOn.ResourceIdReferenceContent}, '2016-03-01').defaultHostName)]";

        public WebAppServiceUrl(WebAppService appService) : base(appService)
        {
            AppService = appService;
        }
    }
}
