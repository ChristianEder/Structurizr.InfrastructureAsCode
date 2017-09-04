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
        public void Connect<TUsing, TUsed>(ContainerWithInfrastructure<TUsing> usingContainer, ContainerWithInfrastructure<TUsed> usedContainer) where TUsing : ContainerInfrastructure where TUsed : ContainerInfrastructure
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

            configurable.Configure("APPINSIGHTS_INSTRUMENTATIONKEY", InstrumentationKey, false);
            UsedBy.Add(reference);
        }

        public string ResourceIdReference => $"[resourceId('microsoft.insights/components/', '{Name}')]";
    }

    public class ApplicationInsightsInstrumentationKey : ConfigurationValue, IDependentConfigurationValue
    {
        private readonly ApplicationInsights _applicationInsights;

        public ApplicationInsightsInstrumentationKey(ApplicationInsights applicationInsights)
        {
            _applicationInsights = applicationInsights;
        }

        public object Value =>
            $"[reference(resourceId('microsoft.insights/components/', '{_applicationInsights.Name}'), '2015-05-01').InstrumentationKey]";

        public bool IsResolved => true;

        public IHaveResourceId DependsOn => _applicationInsights;

        public override bool Equals(object obj)
        {
            return Equals(obj as ApplicationInsightsInstrumentationKey);
        }

        protected bool Equals(ApplicationInsightsInstrumentationKey other)
        {
            return !ReferenceEquals(null, other) && Equals(_applicationInsights, other._applicationInsights);
        }

        public override int GetHashCode()
        {
            return _applicationInsights?.GetHashCode() ?? 0;
        }
    }
}