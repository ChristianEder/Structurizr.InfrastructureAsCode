using System;
using Structurizr.InfrastructureAsCode.Azure.ARM;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.Azure.Tests.Data.Infrastructure;
using Xunit;

namespace Structurizr.InfrastructureAsCode.Azure.Tests
{
    public class InfrastructureRendererBuilderTests
    {
        [Fact]
        public void ShouldNotFailIfNoRendererIsAvailable()
        {
            var renderer = GivenAnInfrastructureRenderer();

            var resourceRenderer = WhenAResourceRendererGetsResolved(renderer, new InfrastructureWithNoRendererYet());

            Assert.Null(resourceRenderer);
        }

        [Fact]
        public void ShouldPreferConsumerRenderer()
        {
            var renderer = GivenAnInfrastructureRenderer();

            var resourceRenderer = WhenAResourceRendererGetsResolved(renderer, new StorageAccount());

            Assert.NotNull(resourceRenderer);
            Assert.IsType<MockStorageAccountRenderer>(resourceRenderer);
        }


        [Fact]
        public void ShouldFallbackToFrameworkRenderer()
        {
            var renderer = GivenAnInfrastructureRenderer();

            var resourceRenderer = WhenAResourceRendererGetsResolved(renderer, new ServiceBus());

            Assert.NotNull(resourceRenderer);
            Assert.IsType<ServiceBusRenderer>(resourceRenderer);
        }


        [Fact]
        public void ShouldFailIfMultipleRenderersWithSamePriorityExist()
        {
            var renderer = GivenAnInfrastructureRenderer();
            Assert.Throws<RendererResolutionException>(() => WhenAResourceRendererGetsResolved(renderer, new IoTHub()));
        }

        private MockInfrastructureRenderer GivenAnInfrastructureRenderer()
        {
            return new InfrastructureRendererBuilder<MockInfrastructureRenderer>().Build();
        }

        private static IAzureResourceRenderer WhenAResourceRendererGetsResolved(MockInfrastructureRenderer renderer, ContainerInfrastructure infrastructure)
        {
            return renderer.IoC.GetRendererFor(new MockIHaveInfrastructure
            {
                Infrastructure = infrastructure
            });
        }
    }
}