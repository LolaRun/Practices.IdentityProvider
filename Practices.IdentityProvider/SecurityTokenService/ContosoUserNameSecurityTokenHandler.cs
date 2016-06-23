namespace Practices.IdentityProvider {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Xml;

    public class ContosoUserNameSecurityTokenHandler : UserNameSecurityTokenHandler {        
        public override bool CanValidateToken {
            get {
                return true;
            }
        }
        
        public override ReadOnlyCollection<ClaimsIdentity> ValidateToken(SecurityToken token) {
            if (token == null) {
                throw new ArgumentNullException("token");
            }

            UserNameSecurityToken usernameToken = token as UserNameSecurityToken;
            if (usernameToken == null) {
                throw new ArgumentException("usernameToken", "The security token is not a valid username security token.");
            }

            // Validate UNtoken.UserName and UNtoken.Password here.
            var userName = usernameToken.UserName;
            var password = usernameToken.Password;
            
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Upn, userName));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "urn:oasis:names:tc:SAML:1.0:am:password"));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.Now.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime));
            return new List<ClaimsIdentity>(new ClaimsIdentity[] { claimsIdentity }).AsReadOnly();
        }
    }
}