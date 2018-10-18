using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using infrastructurizr.Util;
using Newtonsoft.Json.Linq;

namespace infrastructurizr.Commands.ServicePrincipal
{
    public class NewServicePrincipal : Command
    {
        public override string Name => "new serviceprincipal";

        public override string Description =>
            "Creates a new service principal in your Azure AD along with a certificate for authentication, that is authorized to issue and manage resource group deployments.";

        public StringParameter ServicePrincipalName { get; }

        public CommandParameter<Guid> SubscriptionId { get; }

        public CommandParameter<Guid> TenantId { get; }

        public StringParameter ServicePrincipalCertificatePassword { get; }

        public NewServicePrincipal()
        {
            ServicePrincipalName = new StringParameter("name", "The name of the service principal to be created", true);
            SubscriptionId = new CommandParameter<Guid>(Guid.TryParse, "subscriptionId", "The id of the subscription to create the service principal for", true);
            TenantId = new CommandParameter<Guid>(Guid.TryParse, "tenantId", "The id of the tenant to create the service principal for", true);
            ServicePrincipalCertificatePassword = new StringParameter("password", "The password to be used to protect the service principals certificate. If you do not pass this parameter, a random password will be generated and printed out by this command");
        }

        public override void Execute()
        {
            EnsurePassword();

            Console.WriteLine($"### Checking if the service principal \"{ServicePrincipalName.Value}\" already exists, and creating it if required");

            var message = Powershell.RunScriptOf(this,
                KeyValuePair.Create("$TenantId", TenantId.Value.ToString()),
                KeyValuePair.Create("$ServicePrincipalName", ServicePrincipalName.Value),
                KeyValuePair.Create("$CertPassword", ServicePrincipalCertificatePassword.Value),
                KeyValuePair.Create("$SubscriptionId", SubscriptionId.Value.ToString()));


            var success = ((message["Success"] as JValue)?.Value as bool?).GetValueOrDefault(false);
            var created = success && ((message["Created"] as JValue)?.Value as bool?).GetValueOrDefault(false);
            var path = success && created ? message["CertificateLocation"].ToString() : null;

            if (!success)
            {
                using (new TemporaryConsoleColor(ConsoleColor.Red))
                {
                    Console.WriteLine("#### Getting or creating the service principal failed: " + message["Log"]);
                    return;
                }
            }

            using (new TemporaryConsoleColor(ConsoleColor.Green))
            {
                Console.WriteLine("#### " + message["Log"]);
            }

            if (!created)
            {
                return;
            }

            Console.Write("#### Do you want to install the certificate into your personal certificate store on this machine? [y/N] ");
            if (Console.ReadLine()?.ToLowerInvariant() == "y")
            {
                try
                {
                    var cert = new X509Certificate2(path, ServicePrincipalCertificatePassword.Value, X509KeyStorageFlags.PersistKeySet);
                    var store = new X509Store(StoreName.My);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(cert);
                    using (new TemporaryConsoleColor(ConsoleColor.Green))
                    {
                        Console.WriteLine("#### Certificate installed successfully");
                    }
                }
                catch (Exception e)
                {
                    using (new TemporaryConsoleColor(ConsoleColor.Red))
                    {
                        Console.WriteLine("#### Certificate could not be installed: " + e.Message);
                    }
                }
            }
        }

        private void EnsurePassword()
        {
            if (!ServicePrincipalCertificatePassword.HasValue)
            {
                var random = new Random();
                var characters = Enumerable.Range(48, 57 - 48 + 1)
                    .Concat(Enumerable.Range(65, 90 - 65 + 1))
                    .Concat(Enumerable.Range(97, 122 - 97 + 1))
                    .Select(char.ConvertFromUtf32)
                    .ToArray();
                var password = string.Join("", Enumerable.Range(0, 16).Select(i => characters[random.Next(0, characters.Length)]));
                ServicePrincipalCertificatePassword.Set(password);
            }
        }
    }
}
