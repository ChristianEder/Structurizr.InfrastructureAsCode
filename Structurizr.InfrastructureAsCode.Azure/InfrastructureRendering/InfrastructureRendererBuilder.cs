using System;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using System.Linq;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureRendererBuilder : InfrastructureRendererBuilder<InfrastructureRendererBuilder, InfrastructureRenderer, IAzureInfrastructureEnvironment>
    {
        public InfrastructureRendererBuilder()
        {
            var rendererTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a => a.GetTypes())
               .Where(t => typeof(IAzureResourceRenderer).IsAssignableFrom(t))
               .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
               .ToArray();

            var resourceTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a => a.GetTypes())
               .Where(t => typeof(ContainerInfrastructure).IsAssignableFrom(t))
               .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);

            foreach (var resourceType in resourceTypes)
            {
                var rendererBaseType = typeof(AzureResourceRenderer<>).MakeGenericType(resourceType);
                var rendererType = rendererTypes.FirstOrDefault(t => rendererBaseType.IsAssignableFrom(t));
                if (rendererType != null)
                {
                    Ioc.Register(rendererBaseType, rendererType).AsMultiInstance();
                }
            }
        }
        public InfrastructureRendererBuilder UsingResourceGroups(IResourceGroupTargetingStrategy resourceGroups)
        {
            Ioc.Register(resourceGroups);
            return this;
        }

        public InfrastructureRendererBuilder UsingLocations(IResourceLocationTargetingStrategy locations)
        {
            Ioc.Register(locations);
            return this;
        }

        public InfrastructureRendererBuilder UsingCredentials(IAzureSubscriptionCredentials credentials)
        {
            Ioc.Register(credentials);
            return this;
        }
        public InfrastructureRendererBuilder UsingResourceRenderer<TResource, TRenderer>()
          where TResource : ContainerInfrastructure
          where TRenderer : AzureResourceRenderer<TResource>
        {
            Ioc.Register<AzureResourceRenderer<TResource>, TRenderer>().AsMultiInstance();
            return this;
        }
    }

    public static class InfrastructureRendererBuilderExtensions
    {
        public static InfrastructureRendererBuilder UsingResourceGroupPerEnvironment(this InfrastructureRendererBuilder builder, Func<IInfrastructureEnvironment, string> namingConvention)
        {
            return builder.UsingResourceGroups(new ResourceGroupPerEnvironmentStrategy(namingConvention));
        }

        public static InfrastructureRendererBuilder UsingLocation(this InfrastructureRendererBuilder builder, string location)
        {
            return builder.UsingLocations(new FixedResourceLocationTargetingStrategy(location));
        }
    }
}