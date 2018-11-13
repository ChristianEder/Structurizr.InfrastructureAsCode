namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class PublicIpAddress : ContainerInfrastructure, IHaveResourceId
    {
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent=> $"resourceId('Microsoft.Network/publicIPAddresses', '{Name}')";
    }
}