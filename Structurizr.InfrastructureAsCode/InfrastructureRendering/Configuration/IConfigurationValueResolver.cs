using System;
using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration
{
    public interface IConfigurationValueResolver
    {
        Task<object> Resolve(ConfigurationValue value);
        bool CanResolve(ConfigurationValue value);
    }

    public interface IConfigurationValueResolver<in TValue> : IConfigurationValueResolver
        where TValue : ConfigurationValue
    {
        Task<object> Resolve(TValue value);
        bool CanResolve(TValue value);
    }

    public abstract class ConfigurationValueResolver<TValue> : IConfigurationValueResolver<TValue> where TValue : ConfigurationValue
    {
        bool IConfigurationValueResolver.CanResolve(ConfigurationValue value)
        {
            var v = value as TValue;
            return v != null && CanResolve(v);
        }


        Task<object> IConfigurationValueResolver.Resolve(ConfigurationValue value)
        {
            return Resolve((TValue) value);
        }

        public abstract bool CanResolve(TValue value);
        public abstract Task<object> Resolve(TValue value);
    }

    public static class ConfigurationValueResolverExtensions
    {
        public static IConfigurationValueResolver GetResolverFor(this TinyIoC.TinyIoCContainer ioc,
            ConfigurationValue value)
        {
            var resolverType = typeof(IConfigurationValueResolver<>).MakeGenericType(value.GetType());
            var resolver = ioc.Resolve(resolverType);
            return (IConfigurationValueResolver) resolver;
        }
    }
}
