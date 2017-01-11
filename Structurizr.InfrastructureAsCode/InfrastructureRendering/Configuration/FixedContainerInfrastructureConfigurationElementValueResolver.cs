using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration
{
    public class FixedContainerInfrastructureConfigurationElementValueResolver<T> :
        ContainerInfrastructureConfigurationElementValueResolver<ContainerInfrastructureConfigurationElementValue<T>>
    {
        protected override Task<object> Resolve(ContainerInfrastructureConfigurationElementValue<T> value)
        {
            return Task.FromResult((object)value.Value);
        }
    }
}