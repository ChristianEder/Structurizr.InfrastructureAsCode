using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class JsonDefinedInfrastructure : ContainerInfrastructure
    {
        public JsonDefinedInfrastructure(string template, string parameters = null)
        {
            Template = template;
            Parameters = parameters;
        }

        public string Template { get; }
        public string Parameters { get; }
    }
}
