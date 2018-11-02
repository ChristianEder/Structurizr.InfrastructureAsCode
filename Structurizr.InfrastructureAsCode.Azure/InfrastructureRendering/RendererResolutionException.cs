using System;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public class RendererResolutionException : Exception
    {
        public RendererResolutionException(string message) : base(message)
        {
        }
    }
}
