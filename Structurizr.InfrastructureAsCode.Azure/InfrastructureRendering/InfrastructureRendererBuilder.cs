using System;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;
using System.Linq;
using System.Reflection;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class InfrastructureRendererBuilder<TInfrastructureRenderer> : InfrastructureRendererBuilder<InfrastructureRendererBuilder<TInfrastructureRenderer>, TInfrastructureRenderer, IAzureInfrastructureEnvironment>
        where TInfrastructureRenderer : AzureInfrastructureRenderer
    {
        public InfrastructureRendererBuilder()
        {
            var rendererTypes = RendererTypes();

            var resourceTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a =>
               {
                   try
                   {
                       return a.GetTypes();
                   }
                   catch
                   {
                       return Enumerable.Empty<Type>();
                   }
               })
               .Where(t => typeof(ContainerInfrastructure).IsAssignableFrom(t))
               .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);

            foreach (var resourceType in resourceTypes)
            {
                var rendererBaseType = typeof(AzureResourceRenderer<>).MakeGenericType(resourceType);

                var matchingRendererTypes = RenderersFor(rendererTypes, rendererBaseType);

                var rendererType = matchingRendererTypes == null
                    ? null
                    : matchingRendererTypes.Length > 1
                        ? typeof(FailingDueToAmbiguityAzureResourceRenderer<,>).MakeGenericType(resourceType, typeof(TInfrastructureRenderer))
                        : matchingRendererTypes.Single();

                if (rendererType != null)
                {
                    Ioc.Register(rendererBaseType, rendererType).AsMultiInstance();
                }
            }
        }

        public InfrastructureRendererBuilder<TInfrastructureRenderer> UsingResourceGroups(IResourceGroupTargetingStrategy resourceGroups)
        {
            Ioc.Register(resourceGroups);
            return this;
        }

        public InfrastructureRendererBuilder<TInfrastructureRenderer> UsingLocations(IResourceLocationTargetingStrategy locations)
        {
            Ioc.Register(locations);
            return this;
        }

        public InfrastructureRendererBuilder<TInfrastructureRenderer> UsingCredentials(IAzureSubscriptionCredentials credentials)
        {
            Ioc.Register(credentials);
            return this;
        }
        public InfrastructureRendererBuilder<TInfrastructureRenderer> UsingResourceRenderer<TResource, TRenderer>()
          where TResource : ContainerInfrastructure
          where TRenderer : AzureResourceRenderer<TResource>
        {
            Ioc.Register<AzureResourceRenderer<TResource>, TRenderer>().AsMultiInstance();
            return this;
        }

        internal static Type[] RenderersFor(Type resourceType)
        {
            var rendererBaseType = typeof(AzureResourceRenderer<>).MakeGenericType(resourceType);
            return RenderersFor(RendererTypes(), rendererBaseType);
        }

        private static Type[] RenderersFor(Type[] rendererTypes, Type rendererBaseType)
        {
            return rendererTypes
                .Where(rendererBaseType.IsAssignableFrom)
                .GroupBy(RendererPriority)
                .OrderByDescending(p => p.Key)
                .FirstOrDefault()?.ToArray();
        }

        private static Type[] RendererTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                })
                .Where(t => typeof(IAzureResourceRenderer).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .ToArray();
        }

        private static int RendererPriority(Type rendererType)
        {
            if (rendererType.Assembly == Assembly.GetEntryAssembly())
            {
                return 3;
            }

            if (rendererType.Assembly == typeof(InfrastructureRendererBuilder<>).Assembly)
            {
                return 1;
            }

            return 2;
        }
    }

    public static class InfrastructureRendererBuilderExtensions
    {
        public static InfrastructureRendererBuilder<TInfrastructureRenderer> UsingResourceGroupPerEnvironment<TInfrastructureRenderer>(this InfrastructureRendererBuilder<TInfrastructureRenderer> builder, Func<IInfrastructureEnvironment, string> namingConvention)
            where TInfrastructureRenderer : AzureInfrastructureRenderer
        {
            return builder.UsingResourceGroups(new ResourceGroupPerEnvironmentStrategy(namingConvention));
        }

        public static InfrastructureRendererBuilder<TInfrastructureRenderer> UsingLocation<TInfrastructureRenderer>(this InfrastructureRendererBuilder<TInfrastructureRenderer> builder, string location)
            where TInfrastructureRenderer : AzureInfrastructureRenderer
        {
            return builder.UsingLocations(new FixedResourceLocationTargetingStrategy(location));
        }
    }

    public class FailingDueToAmbiguityAzureResourceRenderer<T, TRenderer> : AzureResourceRenderer<T>
        where T : ContainerInfrastructure
        where TRenderer : AzureInfrastructureRenderer
    {
        public FailingDueToAmbiguityAzureResourceRenderer()
        {
            var renderers = InfrastructureRendererBuilder<TRenderer>.RenderersFor(typeof(T));
            throw new RendererResolutionException($"Cannot resolve renderer for {typeof(T).Name}, because there is more than 1 renderer type for this resource type available: {string.Join(", ", renderers.Select(r => r.AssemblyQualifiedName))}");
        }

        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<T> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            throw new NotImplementedException();
        }
    }
}