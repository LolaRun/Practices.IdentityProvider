using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Practices.IdentityProvider {
    public partial class _default : System.Web.UI.Page {
        protected void Page_PreRender(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(Request.QueryString["wreply"])) {
                var host = new Uri(HttpUtility.UrlDecode(Request.QueryString["wreply"])).Host;
                var client = Request.UserHostAddress;
            }
            
            FederatedPassiveSecurityTokenServiceOperations.ProcessRequest(
                Request,
                (ClaimsPrincipal)User,
                ContosoSecurityTokenService.Create(),
                Response);
        }
    }
}