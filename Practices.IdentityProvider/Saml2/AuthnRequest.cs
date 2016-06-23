namespace Practices.IdentityProvider.Saml2 {
    using System;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Xml;

    public class AuthnRequest {
        string Id {
            get;
            set;
        }

        string Issuer {
            get {
                return ConfigurationManager.AppSettings["Saml2.IssuerName"];
            }
        }

        X509Certificate2 SigningCertificate {
            get {
                return CertificateUtil.GetCertificate(
                    ConfigurationManager.AppSettings["Saml2.SigningCertificatePath"],
                    ConfigurationManager.AppSettings["Saml2.SigningCertificatePassword"]);
            }
        }

        public XmlDocument XmlDoc {
            get;
            protected set;
        }

        public AuthnRequest() {
            Id = "_" + Guid.NewGuid().ToString();
            using (StringWriter sw = new StringWriter()) {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;
                using (XmlWriter xw = XmlWriter.Create(sw, xws)) {
                    xw.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("ID", Id);
                    xw.WriteAttributeString("Version", "2.0");
                    xw.WriteAttributeString("IssueInstant", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    xw.WriteAttributeString("ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
                    xw.WriteAttributeString("AssertionConsumerServiceURL", "");

                    xw.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteString(Issuer);
                    xw.WriteEndElement();

                    xw.WriteStartElement("samlp", "NameIDPolicy", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified");
                    xw.WriteAttributeString("AllowCreate", "true");
                    xw.WriteEndElement();

                    xw.WriteStartElement("samlp", "RequestedAuthnContext", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("Comparison", "exact");
                    xw.WriteEndElement();

                    xw.WriteStartElement("saml", "AuthnContextClassRef", "urn:oasis:names:tc:SAML:2.0:assertion");
                    xw.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport");
                    xw.WriteEndElement();

                    xw.WriteEndElement();
                }
                this.XmlDoc = new XmlDocument() {
                    PreserveWhitespace = true,
                    XmlResolver = null,
                };
                this.XmlDoc.LoadXml(sw.ToString());
            }

            AddSignedXml();
        }

        void AddSignedXml() {
            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(this.XmlDoc);
            // Add the key to the SignedXml document. 
            signedXml.SigningKey = SigningCertificate.PrivateKey;

            // http://stackoverflow.com/questions/13750343/net-signedxml-signing-xml-with-transform-algorithm-exc-c14n
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            // Create a reference to be signed.
            Reference reference = new Reference() {
                Uri = "#" + Id
            };
            // Add an enveloped transformation to the reference.
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            //canonicalize
            reference.AddTransform(new XmlDsigExcC14NTransform());
            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Add an RSAKeyValue KeyInfo (optional; helps recipient find key to validate).
            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data keyInfoData = new KeyInfoX509Data(SigningCertificate);
            keyInfo.AddClause(keyInfoData);
            signedXml.KeyInfo = keyInfo;

            // Compute the signature.
            signedXml.ComputeSignature();
            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            // Append the element to the XML document.
            this.XmlDoc.DocumentElement.AppendChild(this.XmlDoc.ImportNode(xmlDigitalSignature, true));
        }
    }
}