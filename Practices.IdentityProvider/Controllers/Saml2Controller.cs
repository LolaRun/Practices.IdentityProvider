namespace Practices.IdentityProvider.Controllers {
    using Saml2;
    using System;
    using System.Configuration;
    using System.Text;
    using System.Web.Mvc;

    public class Saml2Controller : Controller {
        public ActionResult Post(string returnUrl) {
            ViewBag.IdpUrl = ConfigurationManager.AppSettings["Saml2.IdpUrl"];
            AuthnRequest request = new AuthnRequest();
            var xmlString = request.XmlDoc.OuterXml;
            var bytes = Encoding.UTF8.GetBytes(xmlString);            
            ViewBag.SamlRequest = Convert.ToBase64String(bytes);
            ViewBag.RelayState = Guid.NewGuid().ToString();
            Session[ViewBag.RelayState] = returnUrl;
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Consumer(string samlResponse, string relayState) {
            var bytes = Convert.FromBase64String(samlResponse);
            var xmlString = Encoding.UTF8.GetString(bytes);
            AuthnResponse respose = new AuthnResponse(xmlString);
            if (respose.IsValid()) {
                var userName = respose.GetNameID();
                var returnUrl = Session[relayState].ToString();
                return RedirectToAction("Login", "Account", new { UserName = userName, ReturnUrl = returnUrl });
            } else {
                return View();
            }
        }
    }
}