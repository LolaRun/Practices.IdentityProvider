namespace Practices.IdentityProvider.Controllers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [Authorize]
    public class HomeController : Controller {
        // GET: Home
        public ActionResult Index() {
            return View();
        }
    }
}