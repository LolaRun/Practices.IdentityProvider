namespace Practices.IdentityProvider.ActiveClient {
    using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Protocols.WSTrust;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    class Program {
        static void Main(string[] args) {
            string idpAddress = "https://idp.contoso.com/SecurityTokenService/Issue.svc/mixed/username";
            string fedAddress = "https://sts.contoso.com/adfs/services/trust/13/IssuedTokenMixedSymmetricBasic256";
            string svcAddress = "https://internalcrm.contoso.com";

            var idpBinding = new UserNameWSTrustBinding() {
                SecurityMode = SecurityMode.TransportWithMessageCredential
            };
            var fedBinding = new IssuedTokenWSTrustBinding(idpBinding, new EndpointAddress(idpAddress)) {
                SecurityMode = SecurityMode.TransportWithMessageCredential,
                //KeyType = SecurityKeyType.SymmetricKey
            };
            var channelFactory = new WSTrustChannelFactory(fedBinding, fedAddress);
            channelFactory.Credentials.UserName.UserName = "justin@contoso.com";
            channelFactory.Credentials.UserName.Password = "pw";
            var request = new RequestSecurityToken {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointReference(svcAddress),
                //TokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11,
                //TokenType = SecurityTokenTypes.Saml,
            };
            var token = channelFactory.CreateChannel().Issue(request);
            //return token;
        }
    }
}