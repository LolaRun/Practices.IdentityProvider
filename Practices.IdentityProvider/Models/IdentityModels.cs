namespace Practices.IdentityProvider.Models {
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    // You can add User data for the user by adding more properties to your User class, 
    // please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {
        public ClaimsIdentity GenerateUserIdentity(ApplicationUserManager manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add user custom claims here
            userIdentity.AddClaim(new Claim(ClaimTypes.Upn, this.UserName));

            return userIdentity;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager) {
            return Task.FromResult(GenerateUserIdentity(manager));
        }
    }

    public class ApplicationUserStore :
        IUserStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser, string>,
        IUserTwoFactorStore<ApplicationUser, string> {

        #region IUserStore

        public Task CreateAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds a user
        /// </summary>
        public Task<ApplicationUser> FindByIdAsync(string userId) {
            return Task.FromResult(new ApplicationUser() {
                Id = userId,
                UserName = userId
            });
        }

        /// <summary>
        /// Find a user by name
        /// </summary>
        public Task<ApplicationUser> FindByNameAsync(string userName) {
            return Task.FromResult(new ApplicationUser() {
                Id = userName,
                UserName = userName
            });
        }

        #endregion

        #region IUserLockoutStore

        /// <summary>
        /// Returns the current number of failed access attempts. This number usually
        //  will be reset whenever the password is verified or the account is locked out.
        /// </summary>
        public Task<int> GetAccessFailedCountAsync(ApplicationUser user) {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns whether the user can be locked out.
        /// </summary>
        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user) {
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled) {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd) {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserTwoFactorStore

        /// <summary>
        /// Returns whether two factor authentication is enabled for the user
        /// </summary>
        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user) {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled) {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose() {

        }

        public static ApplicationUserStore Create() {
            return new ApplicationUserStore();
        }
    }
}

