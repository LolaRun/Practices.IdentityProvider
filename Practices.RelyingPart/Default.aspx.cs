using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Practices.RelyingPart {
    public partial class _Default : Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Create a ClaimsPrincipal object from the current user to work with claims
            ClaimsPrincipal claimsPrincipal = Page.User as ClaimsPrincipal;
            // ClaimsPrincipal.Claims returns a collection of claims that we can query, iterate over
            // or in this case set as a datasource of a GridView control. Lots of flexibility. 
            this.ClaimsGridView.DataSource = claimsPrincipal.Claims;
            this.ClaimsGridView.DataBind();
        }
    }
}