namespace Practices.IdentityProvider {
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Models;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// UserManager for users where the primary key for the User is of type string.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser> {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store) {
        }

        /// <summary>
        /// Returns true if the password is valid for the user.
        /// </summary>
        public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password) {
            // custom logic code...
            return Task.FromResult(true);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) {
            var manager = new ApplicationUserManager(context.Get<ApplicationUserStore>());
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager) {
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator {
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            return manager;
        }
    }

    /// <summary>
    /// Manages Sign In operations for users
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string> {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user) {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context) {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
