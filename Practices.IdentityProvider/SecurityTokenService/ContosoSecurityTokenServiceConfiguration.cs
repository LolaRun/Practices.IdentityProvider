namespace Practices.IdentityProvider {
    using System;
    using System.Configuration;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Configuration;
    using System.IdentityModel.Metadata;
    using System.IdentityModel.Protocols.WSTrust;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Web;
    using System.Xml;

    public class ContosoSecurityTokenServiceConfiguration: SecurityTokenServiceConfiguration {
        static string IssuerName {
            get {
                return ConfigurationManager.AppSettings["WSTrust.IssuerName"];
            }
        }

        static X509Certificate2 SigningCertificate {
            get {
                return CertificateUtil.GetCertificate(
                    ConfigurationManager.AppSettings["WSTrust.SigningCertificatePath"],
                    ConfigurationManager.AppSettings["WSTrust.SigningCertificatePassword"]);
            }
        }

        public ContosoSecurityTokenServiceConfiguration()
            : base(IssuerName, new X509SigningCredentials(SigningCertificate)) {
            this.SecurityTokenService = typeof(ContosoSecurityTokenService);
            this.ServiceCertificate = SigningCertificate;
        }

        public MetadataBase GetFederationMetadata() {
            // metadata document
            EntityDescriptor entityDescriptor = new EntityDescriptor(new EntityId(this.TokenIssuerName)) {
                SigningCredentials = this.SigningCredentials,
            };

            SecurityTokenServiceDescriptor roleDescriptors = new SecurityTokenServiceDescriptor();            
            // signing key
            KeyDescriptor signingKey = new KeyDescriptor(this.SigningCredentials.SigningKeyIdentifier) {
                Use = KeyType.Signing
            };
            roleDescriptors.Keys.Add(signingKey);

            // Add a collection of offered claims
            roleDescriptors.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Upn, "UPN", "User Principal Name"));

            var endpoint = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
            
            var activeEndpoint = new EndpointReference(endpoint + "/SecurityTokenService/Issue.svc");
            var passiveEndpoint = new EndpointReference(endpoint + "/SecurityTokenService/Issue");

            // add the security token service endpoints
            roleDescriptors.SecurityTokenServiceEndpoints.Add(activeEndpoint);

            // add the passive requestor endpoints            
            roleDescriptors.PassiveRequestorEndpoints.Add(passiveEndpoint);

            // supported protocols            
            roleDescriptors.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            entityDescriptor.RoleDescriptors.Add(roleDescriptors);

            // serialize 
            //var serializer = new MetadataSerializer();
            //XElement federationMetadata = null;

            //using (var stream = new MemoryStream()) {
            //    serializer.WriteMetadata(stream, entityDescriptor);
            //    stream.Flush();
            //    stream.Seek(0, SeekOrigin.Begin);

            //    XmlReaderSettings readerSettings = new XmlReaderSettings {
            //        DtdProcessing = DtdProcessing.Prohibit, // prohibit DTD processing
            //        XmlResolver = null, // disallow opening any external resources
            //        // no need to do anything to limit the size of the input, given the input is crafted internally and it is of small size
            //    };

            //    XmlReader xmlReader = XmlTextReader.Create(stream, readerSettings);
            //    federationMetadata = XElement.Load(xmlReader);
            //}

            return entityDescriptor;
        }

        /// <summary>
        /// Create a reader to provide simulated Metadata endpoint configuration element
        /// </summary>
        /// <param name="activeSTSUrl">The active endpoint URL.</param>
        XmlDictionaryReader GetMetadataReader(string activeSTSUrl) {
            MetadataSet metadata = new MetadataSet();
            MetadataReference mexReferece = new MetadataReference(new EndpointAddress(activeSTSUrl + "/mex"), AddressingVersion.WSAddressing10);
            MetadataSection refSection = new MetadataSection(MetadataSection.MetadataExchangeDialect, null, mexReferece);
            metadata.MetadataSections.Add(refSection);

            byte[] metadataSectionBytes;
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(stringBuilder)) {
                using (XmlTextWriter textWriter = new XmlTextWriter(stringWriter)) {
                    metadata.WriteTo(textWriter);
                    textWriter.Flush();
                    stringWriter.Flush();
                    metadataSectionBytes = stringWriter.Encoding.GetBytes(stringBuilder.ToString());
                }
            }

            return XmlDictionaryReader.CreateTextReader(metadataSectionBytes, XmlDictionaryReaderQuotas.Max);
        }
    }
}