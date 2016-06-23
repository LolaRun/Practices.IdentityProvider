namespace Practices.IdentityProvider {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //FederatedAuthentication.WSFederationAuthenticationModule.AuthorizationFailed += new EventHandler<AuthorizationFailedEventArgs>(WSFederationAuthenticationModule_AuthorizationFailed);
            //FederatedAuthentication.WSFederationAuthenticationModule.RedirectingToIdentityProvider += new EventHandler<RedirectingToIdentityProviderEventArgs>(WSFederationAuthenticationModule_RedirectingToIdentityProvider);
            //FederatedAuthentication.WSFederationAuthenticationModule.SecurityTokenReceived += new EventHandler<SecurityTokenReceivedEventArgs>(WSFederationAuthenticationModule_SecurityTokenReceived);
            //FederatedAuthentication.WSFederationAuthenticationModule.SecurityTokenValidated += new EventHandler<SecurityTokenValidatedEventArgs>(WSFederationAuthenticationModule_SecurityTokenValidated);
            //FederatedAuthentication.WSFederationAuthenticationModule.SessionSecurityTokenCreated += new EventHandler<SessionSecurityTokenCreatedEventArgs>(WSFederationAuthenticationModule_SessionSecurityTokenCreated);
            //FederatedAuthentication.WSFederationAuthenticationModule.SignedIn += new EventHandler(WSFederationAuthenticationModule_SignedIn);
        }

        //void WSFederationAuthenticationModule_SignedIn(object sender, EventArgs e) {
        //    //Anything that's needed right after succesful session and before hitting the application code goes here
        //    System.Diagnostics.Trace.WriteLine("Handling SignIn event");
        //}

        //void WSFederationAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e) {
        //    //Manipulate session token here, for example, changing its expiration value
        //    System.Diagnostics.Trace.WriteLine("Handling SessionSecurityTokenCreated event");
        //    System.Diagnostics.Trace.WriteLine("Key valid from: " + e.SessionToken.KeyEffectiveTime);
        //    System.Diagnostics.Trace.WriteLine("Key expires on: " + e.SessionToken.KeyExpirationTime);
        //}

        //void WSFederationAuthenticationModule_SecurityTokenValidated(object sender, SecurityTokenValidatedEventArgs e) {
        //    //All vlidation SecurityTokenHandler checks are successful
        //    System.Diagnostics.Trace.WriteLine("Handling SecurityTokenValidated event");
        //}

        //void WSFederationAuthenticationModule_SecurityTokenReceived(object sender, SecurityTokenReceivedEventArgs e) {
        //    //Augment token validation with your cusotm validation checks without invalidating the token.
        //    System.Diagnostics.Trace.WriteLine("Handling SecurityTokenReceived event");
        //}

        //void WSFederationAuthenticationModule_AuthorizationFailed(object sender, AuthorizationFailedEventArgs e) {
        //    //Use this event to report more details regarding the ahorization failure
        //    System.Diagnostics.Trace.WriteLine("Handling AuthorizationFailed event");

        //}

        //void WSFederationAuthenticationModule_RedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e) {
        //    //Use this event to programmatically modify the sign-in message to the STS.
        //    System.Diagnostics.Trace.WriteLine("Handling RedirectingToIdentityProvider event");
        //}
    }
}
