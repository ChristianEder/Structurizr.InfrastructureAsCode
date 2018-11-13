using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class VirtualNetwork : ContainerInfrastructure
    {
        private readonly List<Subnet> _subnets = new List<Subnet>();

        public string Prefix { get; set; }
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Network/virtualNetworks', '{Name}')";

        public IEnumerable<Subnet> Subnets => _subnets;

        public Subnet AddSubnet()
        {
            var subnet = new Subnet(this);
            _subnets.Add(subnet);
            return subnet;
        }
    }

    public class Subnet : IHaveResourceId
    {
        private readonly VirtualNetwork _virtualNetwork;

        public Subnet(VirtualNetwork virtualNetwork)
        {
            _virtualNetwork = virtualNetwork;
        }

        public string Name { get; set; }
        public string Prefix { get; set; }
        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"concat({_virtualNetwork.ResourceIdReferenceContent}, '/subnets/{Name}')";
    }
}