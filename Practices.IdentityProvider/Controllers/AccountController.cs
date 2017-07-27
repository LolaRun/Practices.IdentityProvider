namespace Practices.IdentityProvider.Controllers {
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
        
    [AllowAnonymous]
    public class AccountController : Controller {
        public ApplicationSignInManager SignInManager {
            get {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        
        public ActionResult Login(string returnUrl) {
            var saml2 = false;
            var wechat = true;
            if (saml2) {
                return RedirectToAction("Post", "Saml2", new { ReturnUrl = returnUrl });
            }
            if (wechat) {
                var host = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);
                returnUrl = host + returnUrl;
                return RedirectToAction("Post", "WeChat", new { ReturnUrl = returnUrl });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]        
        public async Task<ActionResult> Login(string userName, string returnUrl) {         
            var result = await SignInManager.PasswordSignInAsync(userName, password: "", isPersistent: false, shouldLockout: false);
            switch (result) {
                case SignInStatus.Success:
                    if (Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View();
            }
        }

        [Authorize]
        public ActionResult LogOff() {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}