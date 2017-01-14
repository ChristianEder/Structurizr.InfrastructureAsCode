using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration
{
    public class FixedConfigurationValueResolver<T> : ConfigurationValueResolver<ConfigurationValue<T>>
    {
        public override bool CanResolve(ConfigurationValue<T> value)
        {
            return true;
        }

        public  override Task<object> Resolve(ConfigurationValue<T> value)
        {
            return Task.FromResult((object)value.Value);
        }
    }
}