using System;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class NoSqlDocumentDatabase : ContainerInfrastructure
    {
        public NoSqlDocumentDatabase(Func<IInfrastructureEnvironment, string> name) : base(name)
        {
        }
    }
}