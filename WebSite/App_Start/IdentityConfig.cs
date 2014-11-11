using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WebSite.Data;
using WebSite.Models;

namespace WebSite
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class AmsUserStore : IUserStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>
    {
        private readonly AmsDb dbContext;

        public AmsUserStore(AmsDb dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Dispose()
        {
        }

        public async Task CreateAsync(ApplicationUser user)
        {
            var person = new Person
            {
                ExternalId = user.ExternalId,
                Name = user.UserName
            };
            dbContext.People.Add(person);
            await dbContext.SaveChangesAsync();
            user.Id = person.Id;
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return Task.FromResult<object>(null);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var person = await dbContext.People.SingleOrDefaultAsync(
                p => p.Id == userId);
            if (person == null)
            {
                return null;
            }

            return new ApplicationUser
            {
                Id = userId,
                UserName = person.Name,
                ExternalId = person.ExternalId
            };
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var person = await dbContext.People.Where(p => p.Name == userName).SingleOrDefaultAsync();
            if (person == null)
            {
                return null;
            }

            return new ApplicationUser
            {
                Id = person.Id,
                UserName = person.Name,
                ExternalId = person.ExternalId
            };
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            return Task.FromResult<object>(null);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            return Task.FromResult("foo");
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new AmsUserStore(context.Get<AmsDb>()));
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

}
