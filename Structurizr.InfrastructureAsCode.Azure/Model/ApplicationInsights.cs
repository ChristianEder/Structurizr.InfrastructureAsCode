using System;
using System.Collections.Generic;
using Structurizr.InfrastructureAsCode.Model.Connectors;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class ApplicationInsights : ContainerInfrastructure, IContainerConnector, IHaveResourceId
    {
        public ApplicationInsights()
        {
            UsedBy = new List<IHaveHiddenLink>();
            InstrumentationKey = new ApplicationInsightsInstrumentationKey(this);
        }

        public ApplicationInsightsInstrumentationKey InstrumentationKey { get; }

        public List<IHaveHiddenLink> UsedBy { get; }

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Insights/components/', '{Name}')";

        void IContainerConnector.Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer)
        {
            if (!ReferenceEquals(this, usedContainer.Infrastructure))
            {
                throw new InvalidOperationException();
            }
            var configurable = ContainerConnector.GetConfigurable(usingContainer);
            var reference = usingContainer.Infrastructure as IHaveHiddenLink;
            if (ReferenceEquals(null, reference))
            {
                throw new InvalidOperationException("A container using an ApplicationInsights has to use an infrastructure implementing the IHaveHiddenLink interface");
            }

            configurable.Configure("APPINSIGHTS_INSTRUMENTATIONKEY", InstrumentationKey);
            UsedBy.Add(reference);
        }

        string IContainerConnector.Technology => "Application Insights SDK";

    }

    public class ApplicationInsightsInstrumentationKey : DependentConfigurationValue<ApplicationInsights>
    {

        public ApplicationInsightsInstrumentationKey(ApplicationInsights applicationInsights)
            : base(applicationInsights)
        {
        }

        /// <summary>
        /// Usually I'd say this should be stored secure (e.g. in the connection strings section of web app configuration settings). But it seems that currently, automatic instrumentation will only pick it up from the app settings, not the connection strings.
        /// </summary>
        public override bool ShouldBeStoredSecure => false;

        public override object Value =>
            $"[reference(resourceId('microsoft.insights/components/', '{DependsOn.Name}'), '2015-05-01').InstrumentationKey]";
    }
}