namespace Practices.IdentityProvider {
    using System;
    using System.IdentityModel;
    using System.IdentityModel.Configuration;
    using System.IdentityModel.Protocols.WSTrust;
    using System.Linq;
    using System.Security.Claims;
    using System.Xml;

    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/hh873304.aspx
    /// https://msdn.microsoft.com/en-us/library/ee748501.aspx 
    /// https://www.microsoftpressstore.com/articles/article.aspx?p=2225067
    /// </summary>
    public class ContosoSecurityTokenService : SecurityTokenService {
        public ContosoSecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : base(configuration) {
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request) {
            if (request.AppliesTo == null) {
                throw new InvalidRequestException("The AppliesTo is null.");
            }

            Scope scope = new Scope(request.AppliesTo.Uri.AbsoluteUri, SecurityTokenServiceConfiguration.SigningCredentials);

            scope.TokenEncryptionRequired = false;
            scope.SymmetricKeyEncryptionRequired = false;

            if (string.IsNullOrEmpty(request.ReplyTo)) {
                //scope.ReplyToAddress = scope.AppliesToAddress;
                scope.ReplyToAddress = ReplyToAdfsAddress(scope.AppliesToAddress);
            } else {
                scope.ReplyToAddress = request.ReplyTo;
            }

            return scope;
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope) {
            if (principal == null) {
                throw new InvalidRequestException("The Caller's Principal is null.");
            }

            ClaimsIdentity outputIdentity = new ClaimsIdentity(principal.Claims);
            var authenticationMethod = (from c in principal.Claims
                                        where c.Type == ClaimTypes.AuthenticationMethod
                                        select c.Value).FirstOrDefault();
            outputIdentity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod ?? request.AuthenticationType ?? "urn:oasis:names:tc:SAML:1.0:am:password"));
            outputIdentity.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.Now.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime));

            //outputIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/appcategory", "00008070"));
            return outputIdentity;
        }
        
        string ReplyToAdfsAddress(string replyToAddress) {
            if (replyToAddress.IndexOf("/adfs/services/trust") > -1) {
                replyToAddress = replyToAddress.Replace("http://", "https://");
                replyToAddress = replyToAddress.Replace("/adfs/services/trust", "/adfs/ls");
            }
            return replyToAddress;
        }
        
        public static ContosoSecurityTokenService Create() {
            return new ContosoSecurityTokenService(new ContosoSecurityTokenServiceConfiguration());
        }
    }
}