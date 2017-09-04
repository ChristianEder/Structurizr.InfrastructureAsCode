using System;
using System.Threading.Tasks;
using TinyIoC;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering.Configuration
{
    public interface IConfigurationValueResolver
    {
        Task<object> Resolve(IConfigurationValue value);
        bool CanResolve(IConfigurationValue value);
    }

    public interface IConfigurationValueResolver<in TValue> : IConfigurationValueResolver
        where TValue : IConfigurationValue
    {
        Task<object> Resolve(TValue value);
        bool CanResolve(TValue value);
    }

    public abstract class ConfigurationValueResolver<TValue> : IConfigurationValueResolver<TValue> where TValue : class, IConfigurationValue
    {
        bool IConfigurationValueResolver.CanResolve(IConfigurationValue value)
        {
            var v = value as TValue;
            return v != null && CanResolve(v);
        }


        Task<object> IConfigurationValueResolver.Resolve(IConfigurationValue value)
        {
            return Resolve((TValue) value);
        }

        public abstract bool CanResolve(TValue value);
        public abstract Task<object> Resolve(TValue value);
    }

    public static class ConfigurationValueResolverExtensions
    {
        public static IConfigurationValueResolver GetResolverFor(this TinyIoC.TinyIoCContainer ioc,
            IConfigurationValue value)
        {
            var resolverType = typeof(IConfigurationValueResolver<>).MakeGenericType(value.GetType());
            try
            {
                var resolver = ioc.Resolve(resolverType);
                return (IConfigurationValueResolver)resolver;
            }
            catch (TinyIoCResolutionException e)
            {
                Console.WriteLine(e);
                return null;
                throw;
            }
        }
    }
}
