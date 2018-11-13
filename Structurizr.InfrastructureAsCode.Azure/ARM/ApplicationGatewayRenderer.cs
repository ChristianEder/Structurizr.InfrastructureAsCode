using System.Linq;
using Newtonsoft.Json.Linq;
using Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering;
using Structurizr.InfrastructureAsCode.Azure.Model;

namespace Structurizr.InfrastructureAsCode.Azure.ARM
{
    public class ApplicationGatewayRenderer : AzureResourceRenderer<ApplicationGateway>
    {
        protected override void Render(AzureDeploymentTemplate template, IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure,
            IAzureInfrastructureEnvironment environment, string resourceGroup, string location)
        {
            var gateway = Template(
                "Microsoft.Network/applicationGateways",
                elementWithInfrastructure.Infrastructure.Name,
                location,
                "2018-08-01");

            gateway["zones"] = new JArray();

            gateway["dependsOn"] = new JArray(
                elementWithInfrastructure.Infrastructure.PublicIpAddress.Infrastructure.ResourceIdReference,
                elementWithInfrastructure.Infrastructure.VirtualNetwork.Infrastructure.ResourceIdReference);

            gateway["properties"] = new JObject
            {
                ["sku"] = Sku(elementWithInfrastructure, environment),
                ["gatewayIPConfigurations"] = GatewayIPConfigurations(elementWithInfrastructure, environment),
                ["frontendIPConfigurations"] = FrontendIPConfigurations(elementWithInfrastructure, environment),
                ["frontendPorts"] = FrontendPorts(elementWithInfrastructure, environment),
                ["probes"] = Probes(elementWithInfrastructure, environment),
                ["backendAddressPools"] = BackendPools(elementWithInfrastructure, environment),
                ["backendHttpSettingsCollection"] = BackendHttpSettings(elementWithInfrastructure, environment),
                ["httpListeners"] = HttpListeners(elementWithInfrastructure, environment),
                ["urlPathMaps"] = UrlPathMaps(elementWithInfrastructure, environment),
                ["requestRoutingRules"] = RequestRoutingRules(elementWithInfrastructure, environment),
                ["webApplicationFirewallConfiguration"] = WebApplicationFirewallConfiguration(elementWithInfrastructure, environment)
            };

            template.Resources.Add(PostProcess(gateway));
        }

        protected virtual JObject Sku(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JObject
            {
                ["name"] = "WAF_Medium",
                ["tier"] = "WAF",
                ["capacity"] = "2"
            };
        }

        protected virtual JArray GatewayIPConfigurations(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(new JObject
            {
                ["name"] = "appGatewayIpConfig",
                ["properties"] = new JObject
                {
                    ["subnet"] = new JObject
                    {
                        ["id"] = elementWithInfrastructure.Infrastructure.DefaultSubnet.ResourceIdReference
                    }
                }
            });
        }

        protected virtual JArray FrontendIPConfigurations(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(
                elementWithInfrastructure.Infrastructure.FrontendIps.Select(i => new JObject
                {
                    ["name"] = i.Name,
                    ["properties"] = new JObject
                    {
                        ["PublicIPAddress"] = new JObject
                        {
                            ["id"] = i.PublicIpAddress.ResourceIdReference
                        }
                    }
                }
                ));
        }

        protected virtual JArray FrontendPorts(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.FrontendPorts.Select(p => new JObject
            {
                ["name"] = p.Name,
                ["properties"] = new JObject
                {
                    ["Port"] = p.Port
                }
            }));
        }

        protected virtual JArray Probes
            (IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.Probes.Select(p => new JObject
            {
                ["name"] = p.Name,
                ["properties"] = new JObject
                {
                    ["protocol"] = p.Protocol,
                    ["path"] = p.Path,
                    ["interval"] = p.Interval,
                    ["timeout"] = p.Timeout,
                    ["unhealthyThreshold"] = p.UnhealthyThreshold,
                    ["pickHostNameFromBackendHttpSettings"] = true,
                    ["match"] = new JObject
                    {
                        ["statusCodes"] = new JArray(p.MatchStatusCodes)
                    }
                }
            }));
        }

        protected virtual JArray BackendPools(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.BackendPools.Select(p => new JObject
            {
                ["name"] = p.Name,
                ["properties"] = new JObject
                {
                    ["BackendAddresses"] = new JArray(p.Addresses.Select(a => new JObject
                    {
                        [a.Type] = a.Address
                    }))
                }
            }));
        }

        protected virtual JArray BackendHttpSettings(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.BackendHttpSettings.Select(p => new JObject
            {
                ["name"] = p.Name,
                ["properties"] = new JObject
                {
                    ["Protocol"] = p.Protocol,
                    ["Port"] = p.Port,
                    ["CookieBasedAffinity"] = p.CookieBasedAffinity,
                    ["path"] = p.OverrideBackendPath,
                    ["pickHostNameFromBackendAddress"] = true,
                    ["probe"] = new JObject
                    {
                        ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/probes/{p.Probe.Name}')]"
                    }
                }
            }));
        }
        protected virtual JArray HttpListeners(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.Listeners.Select(p =>
            {
                var properties = new JObject
                {
                    ["FrontendIPConfiguration"] = new JObject
                    {
                        ["Id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/frontendIPConfigurations/{p.Ip.Name}')]"
                    },
                    ["FrontendPort"] = new JObject
                    {
                        ["Id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/frontendPorts/{p.Port.Name}')]"
                    },
                    ["Protocol"] = p.Protocol
                };

                if (!string.IsNullOrEmpty(p.HostName))
                {
                    properties["HostName"] = p.HostName;
                }
                if (p is ApplicationGateway.HttpsListener h)
                {
                    properties["RequireServerNameIndication"] = h.RequireServerNameIndication.ToString().ToLowerInvariant();
                }

                return new JObject
                {
                    ["name"] = p.Name,
                    ["properties"] = properties
                };
            }));
        }

        protected virtual JArray UrlPathMaps(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.UrlPathMaps.Select(u => new JObject
            {
                ["name"] = u.Name,
                ["properties"] = new JObject
                {
                    ["defaultBackendAddressPool"] = new JObject
                    {
                        ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/backendAddressPools/{u.DefaultBackendPool.Name}')]"
                    },
                    ["defaultBackendHttpSettings"] = new JObject
                    {
                        ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/backendHttpSettingsCollection/{u.DefaultBackendHttpSetting.Name}')]"
                    },
                    ["pathRules"] = new JArray(u.Rules.Select(rule => UrlPathMapPathRule(rule, elementWithInfrastructure)))

                }
            }));
        }

        protected virtual JObject UrlPathMapPathRule(ApplicationGateway.UrlPathMap.PathRule rule,
            IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure)
        {
            return new JObject
            {
                ["name"] = rule.Name,
                ["properties"] = new JObject
                {
                    ["paths"] = new JArray(rule.Path),
                    ["backendAddressPool"] = new JObject
                    {
                        ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/backendAddressPools/{rule.BackendPool.Name}')]"
                    },
                    ["backendHttpSettings"] = new JObject
                    {
                        ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/backendHttpSettingsCollection/{rule.BackendHttpSetting.Name}')]"
                    }
                }
            };
        }

        protected virtual JArray RequestRoutingRules(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JArray(elementWithInfrastructure.Infrastructure.Rules.Select(r => RequestRoutingRule(elementWithInfrastructure, r)));
        }

        private JObject RequestRoutingRule(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, ApplicationGateway.Rule rule)
        {
            var properties = new JObject
            {
                ["RuleType"] = rule.Type,
                ["httpListener"] = new JObject
                {
                    ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/httpListeners/{rule.Listener.Name}')]"
                }
            };

            if (rule is ApplicationGateway.BasicForwardRule b)
            {
                properties["backendAddressPool"] = new JObject
                {
                    ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/backendAddressPools/{b.BackendPool.Name}')]"
                };
                properties["backendHttpSettings"] = new JObject
                {
                    ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/backendHttpSettingsCollection/{b.BackendHttpSetting.Name}')]"
                };
            }

            if (rule is ApplicationGateway.PathBasedRule p)
            {
                properties["urlPathMap"] = new JObject
                {
                    ["id"] = $"[concat({elementWithInfrastructure.Infrastructure.ResourceIdReferenceContent}, '/urlPathMaps/{p.UrlPathMap.Name}')]"
                };
            }

            return new JObject
            {
                ["Name"] = rule.Name,
                ["properties"] = properties
            };
        }

        protected virtual JObject WebApplicationFirewallConfiguration(IHaveInfrastructure<ApplicationGateway> elementWithInfrastructure, IAzureInfrastructureEnvironment environment)
        {
            return new JObject
            {
                ["enabled"] = true,
                ["firewallMode"] = "Detection",
                ["ruleSetType"] = "OWASP",
                ["ruleSetVersion"] = "3.0"
            };
        }
    }
}