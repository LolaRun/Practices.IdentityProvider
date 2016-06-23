namespace Practices.IdentityProvider {
    using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    public class ContosoSecurityTokenServiceHostFactory : ServiceHostFactory {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses) {
            var config = new ContosoSecurityTokenServiceConfiguration();
            var host = new WSTrustServiceHost(config, baseAddresses);

            ServiceMetadataBehavior serviceMetadata = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (null == serviceMetadata) {
                serviceMetadata = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(serviceMetadata);
            }
            serviceMetadata.HttpGetEnabled = true;
            serviceMetadata.HttpsGetEnabled = true;
            // add behavior for load balancing support
            host.Description.Behaviors.Add(new UseRequestHeadersForMetadataAddressBehavior());

            // modify address filter mode for load balancing
            var serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            serviceBehavior.AddressFilterMode = AddressFilterMode.Any;

            ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            // if not found - add behavior with setting turned on 
            if (debug == null) {
                host.Description.Behaviors.Add(
                     new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            } else {
                // make sure setting is turned ON
                if (!debug.IncludeExceptionDetailInFaults) {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }

            //host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            // Set the service's X509Certificate to protect the messages.
            //host.Credentials.ServiceCertificate.Certificate = CertificateUtil.GetCertificate("Certificates\\www.contoso.com.pfx", "pass@word1");
            //host.Credentials.ServiceCertificate.Certificate = CertificateUtil.GetCertificate(ConfigurationManager.AppSettings["WSTrust.SigningCertificatePath"], ConfigurationManager.AppSettings["WSTrust.SigningCertificatePassword"]);
            host.Credentials.ServiceCertificate.Certificate = config.ServiceCertificate;
            host.AddServiceEndpoint(typeof(IWSTrust13SyncContract),
                new UserNameWSTrustBinding(SecurityMode.Message), "message/username");

            host.AddServiceEndpoint(typeof(IWSTrust13SyncContract),
                new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential), "mixed/username");

            return host;
        }
    }
}