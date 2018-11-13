using System;
using System.Collections.Generic;
using System.Linq;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public class ApplicationGateway : ContainerInfrastructure, IHaveResourceId
    {
        public ApplicationGateway()
        {
            VirtualNetwork = new SubInfrastructure<VirtualNetwork>(new VirtualNetwork
            {
                Prefix = "10.1.0.0/16",
            });

            DefaultSubnet = VirtualNetwork.Infrastructure.AddSubnet();
            DefaultSubnet.Name = "default";
            DefaultSubnet.Prefix = "10.1.0.0/24";

            PublicIpAddress = new SubInfrastructure<PublicIpAddress>(new PublicIpAddress());

            FrontendIps = new List<FrontendIpConfiguration>
            {
                new FrontendIpConfiguration("appGatewayFrontendIP", PublicIpAddress.Infrastructure)
            };

            FrontendPorts = new List<FrontendPort>
            {
                new FrontendPort("appGatewayFrontendPort", 80)
            };
        }

        protected override void OnNameChanged(string oldName, string newName)
        {
            base.OnNameChanged(oldName, newName);
            VirtualNetwork.Infrastructure.Name = $"{newName}-vnet";
            PublicIpAddress.Infrastructure.Name = $"{newName}-ip";
        }

        public IDefineFrontend Expose => new DefineApplicationGatewayFrontend(this);

        public string ResourceIdReference => $"[{ResourceIdReferenceContent}]";
        public string ResourceIdReferenceContent => $"resourceId('Microsoft.Network/applicationGateways', '{Name}')";

        public Subnet DefaultSubnet { get; }
        public SubInfrastructure<VirtualNetwork> VirtualNetwork { get; }
        public SubInfrastructure<PublicIpAddress> PublicIpAddress { get; }

        public List<FrontendIpConfiguration> FrontendIps { get; }

        public List<FrontendPort> FrontendPorts { get; }

        public List<Listener> Listeners { get; } = new List<Listener>();
        public List<Probe> Probes { get; } = new List<Probe>();
        public List<BackendPool> BackendPools { get; } = new List<BackendPool>();
        public List<BackendHttpSetting> BackendHttpSettings { get; } = new List<BackendHttpSetting>();
        public List<Rule> Rules { get; } = new List<Rule>();
        public List<UrlPathMap> UrlPathMaps { get; } = new List<UrlPathMap>();

        public class FrontendIpConfiguration
        {
            public FrontendIpConfiguration(string name, PublicIpAddress publicIpAddress)
            {
                Name = name;
                PublicIpAddress = publicIpAddress;
            }

            public string Name { get; }
            public PublicIpAddress PublicIpAddress { get; }
        }

        public class FrontendPort
        {
            public FrontendPort(string name, int port)
            {
                Name = name;
                Port = port;
            }

            public string Name { get; }
            public int Port { get; }
        }

        public abstract class Listener
        {
            protected Listener(string name, FrontendIpConfiguration ip, FrontendPort port)
            {
                Name = name;
                Ip = ip;
                Port = port;
            }

            public FrontendIpConfiguration Ip { get; }

            public FrontendPort Port { get; }

            public abstract string Protocol { get; }
            public string HostName { get; set; }
            public string Name { get; set; }
        }

        public class HttpListener : Listener
        {
            public override string Protocol => "Http";

            public HttpListener(string name, FrontendIpConfiguration ip, FrontendPort port) : base(name, ip, port)
            {
            }
        }

        public class HttpsListener : Listener
        {
            public override string Protocol => "Https";
            public bool RequireServerNameIndication { get; set; }

            public HttpsListener(string name, FrontendIpConfiguration ip, FrontendPort port) : base(name, ip, port)
            {
            }
        }

        public class Probe : IEquatable<Probe>
        {
            public Probe(string name, string protocol, string path, int interval, int timeout, int unhealthyThreshold, string matchStatusCodes)
            {
                Name = name;
                Path = path;
                Interval = interval;
                Timeout = timeout;
                UnhealthyThreshold = unhealthyThreshold;
                MatchStatusCodes = matchStatusCodes;
            }

            public string Name { get; }
            public string Path { get; }
            public int Interval { get; }
            public int Timeout { get; }
            public int UnhealthyThreshold { get; }
            public string MatchStatusCodes { get; }
            public string Protocol { get; }


            public bool Equals(Probe other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name) && string.Equals(Path, other.Path) && Interval == other.Interval && Timeout == other.Timeout && UnhealthyThreshold == other.UnhealthyThreshold && string.Equals(MatchStatusCodes, other.MatchStatusCodes) && string.Equals(Protocol, other.Protocol);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Probe) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Interval;
                    hashCode = (hashCode * 397) ^ Timeout;
                    hashCode = (hashCode * 397) ^ UnhealthyThreshold;
                    hashCode = (hashCode * 397) ^ (MatchStatusCodes != null ? MatchStatusCodes.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Protocol != null ? Protocol.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public class BackendPool : IEquatable<BackendPool>
        {
            public string Name { get; set; }

            public List<BackendPoolAddress> Addresses { get; } = new List<BackendPoolAddress>();

            public bool Equals(BackendPool other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name) && Addresses.Count == other.Addresses.Count && Addresses.TrueForAll(a => other.Addresses.Any(o => Equals(a, o)));
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((BackendPool)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Addresses != null ? Addresses.GetHashCode() : 0);
                }
            }
        }

        public class BackendHttpSetting : IEquatable<BackendHttpSetting>
        {
            public string Name { get; set; }
            public int Port { get; set; }
            public string Protocol { get; set; }
            public string CookieBasedAffinity { get; set; }

            public string OverrideBackendPath { get; set; }
            public Probe Probe { get; set; }

            public bool Equals(BackendHttpSetting other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name) && Port == other.Port && string.Equals(Protocol, other.Protocol) && string.Equals(CookieBasedAffinity, other.CookieBasedAffinity) && string.Equals(OverrideBackendPath, other.OverrideBackendPath) && Equals(Probe, other.Probe);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((BackendHttpSetting)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Port;
                    hashCode = (hashCode * 397) ^ (Protocol != null ? Protocol.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (CookieBasedAffinity != null ? CookieBasedAffinity.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (OverrideBackendPath != null ? OverrideBackendPath.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Probe != null ? Probe.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public class BackendPoolAddress : IEquatable<BackendPoolAddress>
        {
            public BackendPoolAddress(AppService appService)
            {
                Type = "fqdn";
                Address = appService.Url.DefaultHostName;
            }
            public string Type { get; }
            public string Address { get; }

            public bool Equals(BackendPoolAddress other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Type, other.Type) && string.Equals(Address, other.Address);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((BackendPoolAddress)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                }
            }
        }

        public abstract class Rule
        {
            protected Rule(string name, Listener listener)
            {
                Name = name;
                Listener = listener;
            }

            public string Name { get; }
            public abstract string Type { get; }

            public Listener Listener { get; }
        }

        public abstract class BasicRule : Rule
        {
            public override string Type => "Basic";

            protected BasicRule(string name, Listener listener) : base(name, listener)
            {
            }
        }

        public class BasicForwardRule : BasicRule
        {
            public BackendHttpSetting BackendHttpSetting { get; }
            public BackendPool BackendPool { get; }

            public BasicForwardRule(string name, Listener listener, BackendHttpSetting backendHttpSetting, BackendPool backendPool) : base(name, listener)
            {
                BackendHttpSetting = backendHttpSetting;
                BackendPool = backendPool;
            }
        }

        public class PathBasedRule : Rule
        {
            public UrlPathMap UrlPathMap { get; }

            public PathBasedRule(string name, Listener listener, UrlPathMap urlPathMap) : base(name, listener)
            {
                UrlPathMap = urlPathMap;
            }

            public override string Type => "PathBasedRouting";
        }

        public class UrlPathMap
        {
            private readonly List<PathRule> _rules = new List<PathRule>();
            public string Name { get; }
            public BackendHttpSetting DefaultBackendHttpSetting { get; }
            public BackendPool DefaultBackendPool { get; }

            public IEnumerable<PathRule> Rules => _rules;

            public UrlPathMap(string name, BackendHttpSetting defaultBackendHttpSetting, BackendPool defaultBackendPool)
            {
                Name = name;
                DefaultBackendHttpSetting = defaultBackendHttpSetting;
                DefaultBackendPool = defaultBackendPool;
            }

            public UrlPathMap WithRule(string name, string path, BackendHttpSetting backendHttpSetting,
                BackendPool backendPool)
            {
                _rules.Add(new PathRule(name, path, backendHttpSetting, backendPool));
                return this;
            }

            public class PathRule
            {
                public PathRule(string name, string path, BackendHttpSetting backendHttpSetting, BackendPool backendPool)
                {
                    Name = name;
                    Path = path;
                    BackendHttpSetting = backendHttpSetting;
                    BackendPool = backendPool;
                }

                public string Name { get; }
                public string Path { get; }
                public BackendHttpSetting BackendHttpSetting { get; }
                public BackendPool BackendPool { get; }

            }
        }


        public interface IDefineFrontend
        {
            IDefineRouting At(string httpListenerName, FrontendIpConfiguration frontendIp, FrontendPort frontendPort);
        }

        public interface IDefineRouting
        {
            IDefineBasicRule ForwardTo(AppService appService, string ruleName = null, string backendPoolName = null);
            IDefinePathBasedRule UsingRoutes(string ruleName);
        }

        public interface IDefineBackendHttpSetting
        {
            IDefineBackendProbe<IDefineRouting> Using(string protocol, int port, string backendHttpSettingName = null);
        }

        public interface IDefineBasicRule : IDefineBackendHttpSetting
        {
        }

        public interface IDefinePathBasedRulePathRule
        {
            IDefineBackendProbe<IDefinePathBasedRule> Using(string protocol, int port, string backendHttpSettingName = null);
        }

        public interface IDefinePathBasedRule
        {
            IDefinePathBasedRulePathRule Route(string routeName, string path, AppService appService, string backendPoolName = null);
            IDefineBackendHttpSetting Default(AppService appService, string backendPoolName = null);
        }


        public interface IDefineBackendProbe<out TReturn>
        {
            IDefineBackendProbe<TReturn> Configure(Action<BackendHttpSetting> configure);
            TReturn WithProbe(string path = "/", int interval = 30, int timeout = 30, int unhealthyThreshold = 3, string matchStatusCodes = "200-399", string probeName = null);
        }

        public class DefineApplicationGatewayFrontend : IDefineFrontend
        {
            private readonly ApplicationGateway _applicationGateway;

            public DefineApplicationGatewayFrontend(ApplicationGateway applicationGateway)
            {
                _applicationGateway = applicationGateway;
            }

            public IDefineRouting At(string httpListenerName, FrontendIpConfiguration frontendIp, FrontendPort frontendPort)
            {
                var httpListener = new HttpListener(httpListenerName, frontendIp, frontendPort);
                _applicationGateway.Listeners.Add(httpListener);
                return new DefineApplicationGatewayRouting(_applicationGateway, httpListener);
            }
        }

        public class DefineApplicationGatewayRouting : IDefineRouting
        {
            private readonly ApplicationGateway _applicationGateway;
            private readonly Listener _listener;

            public DefineApplicationGatewayRouting(ApplicationGateway applicationGateway, Listener listener)
            {
                _applicationGateway = applicationGateway;
                _listener = listener;
            }

            public IDefineBasicRule ForwardTo(AppService appService, string ruleName = null, string backendPoolName = null)
            {
                return new DefineApplicationGatewayBasicRule(_applicationGateway, _listener, appService, ruleName ?? appService.Name, backendPoolName ?? appService.Name, this);
            }

            public IDefinePathBasedRule UsingRoutes(string ruleName)
            {
                return new DefineApplicationGatewayPathBasedRule(_applicationGateway, _listener, ruleName, this);
            }
        }

        public class DefineApplicationGatewayBasicRule : IDefineBasicRule, IDefineBackendProbe<IDefineRouting>
        {
            private readonly ApplicationGateway _applicationGateway;
            private readonly Listener _listener;
            private readonly AppService _appService;
            private readonly string _ruleName;
            private readonly string _backendPoolName;
            private readonly DefineApplicationGatewayRouting _defineRouting;
            private BackendHttpSetting _backendHttpSetting;

            public DefineApplicationGatewayBasicRule(ApplicationGateway applicationGateway, Listener listener,
                AppService appService, string ruleName, string backendPoolName, DefineApplicationGatewayRouting defineRouting)
            {
                _applicationGateway = applicationGateway;
                _listener = listener;
                _appService = appService;
                _ruleName = ruleName;
                _backendPoolName = backendPoolName;
                _defineRouting = defineRouting;
            }

            public IDefineBackendProbe<IDefineRouting> Using(string protocol, int port, string backendHttpSettingName = null)
            {
                _backendHttpSetting = new BackendHttpSetting
                {
                    Name = backendHttpSettingName ?? _appService.Name,
                    Port = port,
                    Protocol = protocol
                };

                return this;
            }

            public IDefineBackendProbe<IDefineRouting> Configure(Action<BackendHttpSetting> configure)
            {
                configure(_backendHttpSetting);
                return this;
            }

            public IDefineRouting WithProbe(string path = "/", int interval = 30, int timeout = 30, int unhealthyThreshold = 3,
                string matchStatusCodes = "200-399", string probeName = null)
            {
                _applicationGateway.Listeners.Add(_listener);
                var probe = new Probe(probeName ?? _appService.Name, _backendHttpSetting.Protocol, path, interval, timeout, unhealthyThreshold, matchStatusCodes);

                if (!_applicationGateway.Probes.Contains(probe))
                {
                    _applicationGateway.Probes.Add(probe);
                }
                else
                {
                    probe = _applicationGateway.Probes.Single(p => Equals(p, probe));
                }

                _backendHttpSetting.Probe = probe;

                if (!_applicationGateway.BackendHttpSettings.Contains(_backendHttpSetting))
                {
                    _applicationGateway.BackendHttpSettings.Add(_backendHttpSetting);
                }
                else
                {
                    _backendHttpSetting = _applicationGateway.BackendHttpSettings.Single(p => Equals(p, _backendHttpSetting));
                }

                var backendPool = new BackendPool
                {
                    Name = _backendPoolName,
                    Addresses = { new BackendPoolAddress(_appService) }
                };

                if (!_applicationGateway.BackendPools.Contains(backendPool))
                {
                    _applicationGateway.BackendPools.Add(backendPool);
                }
                else
                {
                    backendPool = _applicationGateway.BackendPools.Single(p => Equals(p, backendPool));
                }


                _applicationGateway.Rules.Add(new BasicForwardRule(_ruleName, _listener, _backendHttpSetting, backendPool));

                return _defineRouting;
            }
        }
        public class DefineApplicationGatewayPathBasedRule : IDefinePathBasedRule, IDefineBackendHttpSetting, IDefinePathBasedRulePathRule, IDefineBackendProbe<IDefinePathBasedRule>, IDefineBackendProbe<IDefineRouting>
        {
            private readonly ApplicationGateway _applicationGateway;
            private readonly Listener _listener;
            private readonly string _ruleName;
            private readonly DefineApplicationGatewayRouting _defineApplicationGatewayRouting;
            private string _routeName;
            private string _path;
            private AppService _appService;
            private BackendHttpSetting _backendHttpSetting;
            private string _backendPoolName;
            private readonly List<UrlPathMap.PathRule> _rules = new List<UrlPathMap.PathRule>();

            public DefineApplicationGatewayPathBasedRule(ApplicationGateway applicationGateway, Listener listener,
                string ruleName, DefineApplicationGatewayRouting defineApplicationGatewayRouting)
            {
                _applicationGateway = applicationGateway;
                _listener = listener;
                _ruleName = ruleName;
                _defineApplicationGatewayRouting = defineApplicationGatewayRouting;
            }

            public IDefinePathBasedRulePathRule Route(string routeName, string path, AppService appService, string backendPoolName = null)
            {
                _routeName = routeName;
                _path = path;
                _appService = appService;
                _backendPoolName = backendPoolName ?? appService.Name;
                return this;
            }

            public IDefineBackendHttpSetting Default(AppService appService, string backendPoolName = null)
            {
                _appService = appService;
                _backendPoolName = backendPoolName ?? appService.Name;
                return this;
            }

            public IDefineBackendProbe<IDefineRouting> Using(string protocol, int port, string backendHttpSettingName = null)
            {
                CreateBackendHttpSetting(protocol, port, backendHttpSettingName);
                return this;
            }

            IDefineBackendProbe<IDefinePathBasedRule> IDefinePathBasedRulePathRule.Using(string protocol, int port, string backendHttpSettingName)
            {
                CreateBackendHttpSetting(protocol, port, backendHttpSettingName);
                return this;
            }

            IDefineBackendProbe<IDefinePathBasedRule> IDefineBackendProbe<IDefinePathBasedRule>.Configure(Action<BackendHttpSetting> configure)
            {
                configure(_backendHttpSetting);
                return this;
            }

            IDefineBackendProbe<IDefineRouting> IDefineBackendProbe<IDefineRouting>.Configure(Action<BackendHttpSetting> configure)
            {
                configure(_backendHttpSetting);
                return this;
            }

            public IDefinePathBasedRule WithProbe(string path = "/", int interval = 30, int timeout = 30, int unhealthyThreshold = 3,
                string matchStatusCodes = "200-399", string probeName = null)
            {
                var backendPool = AddProbeAndBackendPool(path, interval, timeout, unhealthyThreshold, matchStatusCodes, probeName);
                _rules.Add(new UrlPathMap.PathRule(_routeName, _path, _backendHttpSetting, backendPool));
                return this;
            }

            IDefineRouting IDefineBackendProbe<IDefineRouting>.WithProbe(string path, int interval, int timeout, int unhealthyThreshold,
                string matchStatusCodes, string probeName)
            {
                var backendPool = AddProbeAndBackendPool(path, interval, timeout, unhealthyThreshold, matchStatusCodes, probeName);
                var urlPathMap = new UrlPathMap(_ruleName, _backendHttpSetting, backendPool);
                foreach (var rule in _rules)
                {
                    urlPathMap.WithRule(rule.Name, rule.Path, rule.BackendHttpSetting, rule.BackendPool);
                }

                _applicationGateway.UrlPathMaps.Add(urlPathMap);
                var pathRule = new PathBasedRule(_ruleName, _listener, urlPathMap);
                _applicationGateway.Rules.Add(pathRule);


                return _defineApplicationGatewayRouting;
            }

            private void CreateBackendHttpSetting(string protocol, int port, string backendHttpSettingName)
            {
                _backendHttpSetting = new BackendHttpSetting
                {
                    Protocol = protocol,
                    Port = port,
                    Name = backendHttpSettingName ?? _appService.Name
                };
            }

            private BackendPool AddProbeAndBackendPool(string path, int interval, int timeout, int unhealthyThreshold, string matchStatusCodes, string probeName)
            {
                var probe = new Probe(probeName ?? _appService.Name, _backendHttpSetting.Protocol, path, interval, timeout, unhealthyThreshold, matchStatusCodes);

                if (!_applicationGateway.Probes.Contains(probe))
                {
                    _applicationGateway.Probes.Add(probe);
                }
                else
                {
                    probe = _applicationGateway.Probes.Single(p => Equals(p, probe));
                }

                _backendHttpSetting.Probe = probe;
                if (!_applicationGateway.BackendHttpSettings.Contains(_backendHttpSetting))
                {
                    _applicationGateway.BackendHttpSettings.Add(_backendHttpSetting);
                }
                else
                {
                    _backendHttpSetting = _applicationGateway.BackendHttpSettings.Single(p => Equals(p, _backendHttpSetting));
                }

                var backendPool = new BackendPool
                {
                    Name = _backendPoolName,
                    Addresses = { new BackendPoolAddress(_appService) }
                };

                if (!_applicationGateway.BackendPools.Contains(backendPool))
                {
                    _applicationGateway.BackendPools.Add(backendPool);
                }
                else
                {
                    backendPool = _applicationGateway.BackendPools.Single(p => Equals(p, backendPool));
                }

                return backendPool;
            }
        }
    }
}