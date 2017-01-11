using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration
{
    public abstract class ContainerInfrastructureConfigurationElementValueResolver
    {
        public abstract Task<object> Resolve(ContainerInfrastructureConfigurationElementValue value);
        public abstract bool CanResolve(ContainerInfrastructureConfigurationElementValue value);
    }

    public abstract class ContainerInfrastructureConfigurationElementValueResolver<TValue>
        : ContainerInfrastructureConfigurationElementValueResolver
        where TValue : ContainerInfrastructureConfigurationElementValue
    {
        public override bool CanResolve(ContainerInfrastructureConfigurationElementValue value)
        {
            return value is TValue;
        }

        public override Task<object> Resolve(ContainerInfrastructureConfigurationElementValue value)
        {
            return Resolve((TValue)value);
        }

        protected abstract Task<object> Resolve(TValue value);
    }
}
