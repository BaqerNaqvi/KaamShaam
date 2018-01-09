using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using KaamShaam.AdminServices;
using KaamShaam.LocalModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using KaamShaam.Models;
using KaamShaam.Services;

namespace KaamShaam.Controllers
{
    [System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
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
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return GetLoginStuff();
        }

        private ActionResult GetLoginStuff()
        {
            var vendors = UserServices.GetUserByType("Vendor");
            var cats = CategoryService.GetAllCategories();

            return View(new RegisterPageWraper
            {
                Vendors = vendors,
                Categories = cats
            });
        }

        //
        // POST: /Account/Login
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> Login(RegisterPageWraper model, string returnUrl)
        {
            if (!ModelState.IsValid) 
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "Home/Index";
            }


            var findByEmail = UserManager.FindByEmail(model.LoginViewModel.Email);
            if (findByEmail == null)
            {
                ModelState.AddModelError("", "Invalid login attempt or user does not exist");
                return GetLoginStuff();
            }

           

            var uid = findByEmail.Id;
            var uObj = UserServices.GetUserById(uid);

            if (uObj.Roles.Any(r => r.ToLower().Contains("admin") || r.ToLower().Contains("super admin")))
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return GetLoginStuff();
            }


            if (!uObj.PhoneNumberConfirmed)
            {
                TempData.Add("userId", uObj.Id);
                TempData.Add("userNumber", uObj.Mobile);
                return RedirectToAction("VerifyNumber", "Account");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                {

                    var isApproved = UserAdminService.IsUserApproved(model.LoginViewModel.Email);
                    if (!isApproved)
                    {
                        var mesge = string.IsNullOrEmpty(uObj.Feedback)
                            ? "User is not approved by admin."
                            : uObj.Feedback;
                        ModelState.AddModelError("", mesge);
                        return GetLoginStuff();
                    }
                    SetUserSession(uObj);
                    returnUrl = uObj.Type == "User" ? "/Job/ManageJobs" : "/Job/findJobs";
                     return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.LoginViewModel.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return GetLoginStuff();
            }
        }

        //
        // GET: /Account/VerifyCode
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Register()
        {
            var vendors = UserServices.GetUserByType("Vendor");
            var cats = CategoryService.GetAllCategories();

            return View(new RegisterPageWraper
            {
                Vendors = vendors,
                Categories = cats
            });
        }

        //
        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterPageWraper model)
        {
           if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.RegisterViewModel.Email, Email = model.RegisterViewModel.Email };
                var result = await UserManager.CreateAsync(user, model.RegisterViewModel.Password);
                if (result.Succeeded)
                {
                    // mobile number 
                    var mobile = model.RegisterViewModel.Mobile;
                    mobile = mobile.Substring(1).Replace("-", "");
                    model.RegisterViewModel.Mobile = "92" + mobile;

                    UserServices.AddUserProperties(model.RegisterViewModel,user.Id);
                    var uObj = UserServices.GetUserById(user.Id);
                    SetUserSession(uObj, true);

                        var content = "Hi " + model.RegisterViewModel.FullName +
                                      "!\nYou have been successfully registered as a " + model.RegisterViewModel.Type +
                                      " at KamSham.Pk.";
                        if (model.RegisterViewModel.Type == "Contractor")
                        {
                            content = content + "You will be able to login once we approve your account information.";
                        }
                        content = content + "\n-KamSham Team\n+923084449991";
                        KaamShaam.Services.EmailService.SendEmail(user.Email,"Registration Notification | KamSham.Pk",content);

                    TempData.Add("userId", user.Id);
                    TempData.Add("userNumber", model.RegisterViewModel.Mobile);

                    return RedirectToAction("VerifyNumber", "Account");

                    //if (model.RegisterViewModel.Type == "Contractor")
                    //{
                    //    return RedirectToAction("Welcome", "Account");
                    //}

                    //   await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //   return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            var cats = CategoryService.GetAllCategories();
            model.Categories = cats;
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
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
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Register", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
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
        #endregion

        public void SetUserSession(LocalUser user, bool createImg=false)
        {
            #region Session
            Session["UserName"] = user.FullName;
            Session["Address"] = user.LocationName;
            Session["Type"] = user.Type;
            Session["Score"] = user.Score;
            Session["Votes"] = user.UserRatings.Count;

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                   Request.ApplicationPath.TrimEnd('/') + "/Images/";

            try
            {
                if (createImg)
                {
                    var baseUrlforimg = Server.MapPath("/Images/Profiles/");
                    UserServices.CreateUserAvatar(baseUrlforimg + user.Id);
                }

            }
            catch (Exception)
            {

            }

            var img = baseUrl + "Profiles/" + user.Id + "_110.png";
            Session["Photo"] = AppUtils.Common.ReturnImage(img, "110x110");
            #endregion
        }

        public static string SetImagePath(string userId, string size, string baseUrl)
        {
            var img = baseUrl + "Profiles/" + userId + "_110.png";
            var imggpath = AppUtils.Common.ReturnImage(img, size);
            return imggpath;
        }


        //
        // GET: /Account/Login
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult AdminLogin(string returnUrl)
        {
            var vendors = UserServices.GetUserByType("Vendor");
            var cats = CategoryService.GetAllCategories();

            return View(new RegisterPageWraper
            {
                Vendors = vendors,
                Categories = cats
            });
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Welcome()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> AdminLogin(RegisterPageWraper model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View("AdminLogin");
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "Admin/Stats";
            }


            var findByEmail = UserManager.FindByEmail(model.LoginViewModel.Email);
            if (findByEmail == null)
            {
                ModelState.AddModelError("", "Invalid login attempt or admin does not exist");
                return GetLoginStuff();
            }
            var uid = findByEmail.Id;
            var uObj = UserServices.GetUserById(uid);

            if (uObj.Roles.Any(r => r.ToLower().Contains("user") || r.ToLower().Contains("contractor")))
            {
                ModelState.AddModelError("", "Invalid admin login attempt.");
                return View("AdminLogin");
            }


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.LoginViewModel.Email, model.LoginViewModel.Password, true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        
                        SetUserSession(uObj);
                        // return RedirectToLocal(returnUrl);
                        return RedirectToAction("Stats", "Admin");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.LoginViewModel.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid admin login attempt.");
                    return View("AdminLogin");
            }
        }

        //
        // POST: /Account/LogOff
        public ActionResult AdminLogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("AdminLogin", "Account");
        }

        public bool ChangePassword(KaamShaam.AdminModels.LocalUser usermodel)
        {
            var localUsermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = localUsermanager.FindById(usermodel.Id);
            if (user == null)
            {
                return false;
            }
            user.PasswordHash = localUsermanager.PasswordHasher.HashPassword(usermodel.Password);
            var result = localUsermanager.Update(user);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        // GET: GeneralAdmin
        public ActionResult UpdateUser(AdminModels.LocalUser user)
        {
            AdminService.EditUserByAdmin(user);
            bool result = true;
            if (!string.IsNullOrEmpty(user.Password))
            {
                result = ChangePassword(user);

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Oops()
        {
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult NotFound()
        {
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult VerifyNumber()
        {
            var userId = TempData["userId"].ToString();
            var mobile = TempData["userNumber"].ToString();
            GeneratePhoneCode(userId, mobile);

            var model = new VarifyNumberModel
            {
                UserId = userId,
                Phone = mobile
            };
                return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult VerifyNumberAjax(VarifyNumberModel model)
        {
            if (string.IsNullOrEmpty(model?.Code) || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Phone))
            {
                return Json(new {status=false, message = "Bad Request"}, JsonRequestBehavior.AllowGet);
            }
            var status = UserManager.ChangePhoneNumber(model.UserId, model.Phone, model.Code);
            if (!status.Succeeded)
            {
                return Json(new { status = false, message = "Unable to verify mobile number" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, message = "success" }, JsonRequestBehavior.AllowGet);
        }

        private string GeneratePhoneCode(string userId, string mobile)
        {
            var phoneCode = UserManager.GenerateChangePhoneNumberToken(userId, mobile);
            KaamShaam.Services.EmailService.SendSms(mobile, "Your verification code is : " + phoneCode);
            return phoneCode;
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult ReGeneratePhoneCode(VarifyNumberModel model)
        {
            GeneratePhoneCode(model.UserId, model.Phone);
            return Json(new { status = true, message = "success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSelfPassword(KaamShaam.AdminModels.LocalUser user)
        {
            var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            user.Id = id;
            var result = ChangePassword(user);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}