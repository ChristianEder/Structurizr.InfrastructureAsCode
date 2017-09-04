using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration
{
    public class FixedConfigurationValueResolver<T> : ConfigurationValueResolver<FixedConfigurationValue<T>>
    {
        public override bool CanResolve(FixedConfigurationValue<T> value)
        {
            return true;
        }

        public  override Task<object> Resolve(FixedConfigurationValue<T> value)
        {
            return Task.FromResult((object)value.Value);
        }
    }
}