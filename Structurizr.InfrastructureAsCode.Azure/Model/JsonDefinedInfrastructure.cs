using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class JsonDefinedInfrastructure : ContainerInfrastructure
    {
        public JsonDefinedInfrastructure(string template)
        {
            Template = template;
        }

        public string Template { get; }
    }
}
