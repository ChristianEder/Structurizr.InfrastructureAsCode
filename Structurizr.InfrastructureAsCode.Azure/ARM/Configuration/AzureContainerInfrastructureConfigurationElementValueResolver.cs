using Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration;

namespace Structurizr.InfrastructureAsCode.Azure.ARM.Configuration
{
    public abstract class AzureContainerInfrastructureConfigurationElementValueResolver<TValue>
        : ContainerInfrastructureConfigurationElementValueResolver<TValue>,
        IAzureContainerInfrastructureConfigurationElementValueResolver
        where TValue : ContainerInfrastructureConfigurationElementValue
    {
        protected AzureContainerInfrastructureConfigurationElementValueResolverContext Context;

        public void SetContext(AzureContainerInfrastructureConfigurationElementValueResolverContext context)
        {
            Context = context;
        }
    }
}