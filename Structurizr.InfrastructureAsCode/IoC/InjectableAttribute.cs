using System;

namespace Structurizr.InfrastructureAsCode.IoC
{
    public class InjectableAttribute : Attribute
    {
        public bool Singleton { get; set; } = false;
    }
}
