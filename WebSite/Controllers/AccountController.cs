using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebSite.Data;
using WebSite.Models;

namespace WebSite.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var dbExternalId = GetDbExternalId(loginInfo);
            if (dbExternalId == null)
            {
                return RedirectToAction("Login");
            }

            // This is where we should inspect the external info, work out whether it corresponds
            // to a known user, and then decide what to do. Hack it for now.

            var dbContext = HttpContext.GetOwinContext().Get<AmsDb>();
            Person person = await dbContext.People.SingleOrDefaultAsync(p => p.ExternalId == dbExternalId);

            if (person == null)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.DefaultUserName });
            }

            var user = new ApplicationUser
            {
                Id = person.Id,
                ExternalId = person.ExternalId,
                UserName = person.Name
            };

            ClaimsIdentity userIdentity = await user.GenerateUserIdentityAsync(UserManager);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, userIdentity);

            return RedirectToLocal(returnUrl);
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }


                var dbExternalId = GetDbExternalId(info);
                if (dbExternalId == null)
                {
                    return RedirectToAction("Login");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, ExternalId = dbExternalId };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    ClaimsIdentity userIdentity = await user.GenerateUserIdentityAsync(UserManager);
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, userIdentity);
                    return RedirectToLocal(returnUrl);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private static string GetDbExternalId(ExternalLoginInfo loginInfo)
        {
            string dbExternalId = null;
            switch (loginInfo.Login.LoginProvider)
            {
                case "Facebook":
                case "Twitter":
                    dbExternalId = loginInfo.Login.LoginProvider + ":" + loginInfo.Login.ProviderKey;
                    break;

                default:
                    const string aadPrefix = "https://sts.windows.net/";
                    if (loginInfo.Login.LoginProvider.StartsWith(aadPrefix))
                    {
                        var claims = loginInfo.ExternalIdentity.Claims.ToDictionary(c => c.Type);
                        Claim tenantId, objectId;
                        if (claims.TryGetValue("http://schemas.microsoft.com/identity/claims/tenantid", out tenantId) &&
                            claims.TryGetValue("http://schemas.microsoft.com/identity/claims/objectidentifier",
                                out objectId))
                        {
                            dbExternalId = "Aad:" + tenantId.Value + "," + objectId.Value;
                        }

                        // TODO: One sneaky little thing we don't handle yet:
                        // Because Microsoft Accounts can belong to an AAD, it's possible to get a successful
                        // auth for which you've got no tenant ID. (And possibly no object ID.)
                    }
                    break;
            }
            return dbExternalId;
        }

        #endregion
    }
}