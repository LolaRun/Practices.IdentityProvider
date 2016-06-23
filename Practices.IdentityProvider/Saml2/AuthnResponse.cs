namespace Practices.IdentityProvider.Saml2 {
    using System.Configuration;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Xml;

    public class AuthnResponse {
        X509Certificate2 DecrypingCertificate {
            get {
                var path = ConfigurationManager.AppSettings["Saml2.DecrypingCertificatePath"];
                return CertificateUtil.GetCertificate(path);
            }
        }

        public XmlDocument XmlDoc {
            get;
            protected set;
        }

        public AuthnResponse(string xmlString) {
            this.XmlDoc = new XmlDocument() {
                PreserveWhitespace = true,
                XmlResolver = null,
            };
            this.XmlDoc.LoadXml(xmlString);
        }

        public bool IsValid() {
            XmlNamespaceManager manager = new XmlNamespaceManager(XmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            XmlNodeList nodeList = XmlDoc.SelectNodes("//ds:Signature", manager);

            SignedXml signedXml = new SignedXml(XmlDoc);
            signedXml.LoadXml((XmlElement)nodeList[0]);
            return signedXml.CheckSignature(DecrypingCertificate, true);
        }

        public string GetNameID() {
            XmlNamespaceManager manager = new XmlNamespaceManager(XmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            XmlNode node = XmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", manager);
            return node.InnerText.Trim();
        }
    }
}