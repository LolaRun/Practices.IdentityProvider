namespace Practices.IdentityProvider.Controllers {
    using System.IdentityModel.Metadata;
    using System.IdentityModel.Services;
    using System.IO;
    using System.Security.Claims;
    using System.Text;
    using System.Web.Mvc;
    using System.Xml;

    /// <summary>
    /// http://www.stevesdevbox.com/Blogs/Writing-an-MVC-Security-Token-Service-for-Development
    /// https://blogs.technet.microsoft.com/askpfeplat/2014/11/02/adfs-deep-dive-comparing-ws-fed-saml-and-oauth/
    /// https://blogs.msdn.microsoft.com/besidethepoint/2012/10/17/request-adfs-security-token-with-powershell/
    /// https://blogs.msdn.microsoft.com/mcsuksoldev/2011/08/17/federated-security-how-to-setup-and-call-a-wcf-service-secured-by-adfs-2-0/
    /// </summary>
    [AllowAnonymous]
    public class SecurityTokenServiceController : Controller {
        const string SigninAction = "wsignin1.0";
        const string SignoutAction = "wsignout1.0";
        const string SignoutCleanupAction = "wsignoutcleanup1.0";
        
        public ActionResult Issue() {
            var requestMessage = WSFederationMessage.CreateFromUri(Request.Url);
            switch (requestMessage.Action) {
                case SigninAction:
                    SignInResponseMessage responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(
                        (SignInRequestMessage)requestMessage,
                        (ClaimsPrincipal)User,
                        ContosoSecurityTokenService.Create());
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(
                        responseMessage,
                        System.Web.HttpContext.Current.Response);
                    break;
                case SignoutAction:
                case SignoutCleanupAction:
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(
                        (SignOutRequestMessage)requestMessage,
                        (ClaimsPrincipal)User,
                        null,
                        System.Web.HttpContext.Current.Response);
                    return RedirectToAction("LogOff", "Account");
                default:
                    break;
            }
            return View();
        }
        
        public ActionResult FederationMetadata() {
            var entityDescriptor = new ContosoSecurityTokenServiceConfiguration().GetFederationMetadata();
            var serializer = new MetadataSerializer();
            var settings = new XmlWriterSettings {
                Encoding = Encoding.UTF8
            };
            using (var memoryStream = new MemoryStream()) {
                using (var writer = XmlWriter.Create(memoryStream, settings)) {
                    serializer.WriteMetadata(writer, entityDescriptor);
                    writer.Flush();
                    return Content(Encoding.UTF8.GetString(memoryStream.GetBuffer()), "text/xml");
                }
            }
        }
    }
}