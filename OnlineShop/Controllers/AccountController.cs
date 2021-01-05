using OnlineShop.DAL;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineShop.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserProvider userProvider;
        public AccountController()
        {
            userProvider = new UserProvider();
        }
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signin(RegisterSignin model, string returnUrl)
        {
            if (model.EmailAddress == string.Empty || model.EmailAddress == null)
            {
                ModelState.AddModelError("EmailAddress", "Email address is required");
                return View();
            }
            if (model.Password == string.Empty || model.Password == null)
            {
                ModelState.AddModelError("Password", "Password address is required");
                return View();
            }

            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                string ss = returnUrl;
            }
            var user = userProvider.Authenticate(model.EmailAddress, Encode(model.Password));
            if (user != null)
            {
                Session[CommonConstants.Constants.User] = MapUserToModel(user);
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
              //  return RedirectToAction("index","home");
               return Redirect("~/Home/index");

            }

            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            var user = Session[CommonConstants.Constants.User] as User;
            Response.Cookies[user.EmailAddress].Expires = DateTime.Now.AddDays(-1);
            
            Session[CommonConstants.Constants.User]=null;
            var user2 = Session[CommonConstants.Constants.User] as User;
            return RedirectToAction("Signin");
        }

        public ActionResult Register()
        {
            return View(new RegisterSignin());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterSignin registerUser)
        {
            if (ModelState.IsValid)
            {
                if (!userProvider.IsUserExist(registerUser.EmailAddress))
                {
                    string uniqueId = userProvider.Register(MapUserToDomain(registerUser));

                    try
                    {
                        SendEmail(registerUser.EmailAddress, "Your account has registered successfully");
                    }
                    catch (Exception ex)
                    {
                        TempData[CommonConstants.Constants.ErrorMessage] = "Email sending failed";
                    }

                    var user = userProvider.Authenticate(registerUser.EmailAddress, Encode(registerUser.Password));
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
                        Session[CommonConstants.Constants.User] = MapUserToModel(user);
                        return RedirectToAction("index", "home");
                    }
                }
                TempData["Error"] = "Email address alreay exists. Please try another email address";
                return View(registerUser);
            }
            return View(registerUser);
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(User model)
        {
            if (userProvider.IsUserExist(model.EmailAddress))
            {
                string password = Guid.NewGuid().ToString("d").Substring(1, 8);

                userProvider.UpdatePasswordByEmail(model.EmailAddress, Encode(password));

                try
                {
                    SendEmail(model.EmailAddress, "This is a computer generated password you can login using : " + password);
                    TempData[CommonConstants.Constants.SuccessMessage] = "Email sent successfully";
                }
                catch (Exception ex)
                {
                    TempData[CommonConstants.Constants.ErrorMessage] = "Email sending failed";
                }
            }
            else
            {

            }

            return RedirectToAction("ForgetPassword");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(RegisterSignin model)
        {
            var user = (User)Session[CommonConstants.Constants.User];
            var password = userProvider.GetUserPasswordByEmail(user.EmailAddress);
            bool IsCurrentPass = model.CurrentPassword == Decode(password);

            if (IsCurrentPass)
            {
                userProvider.UpdatePasswordByEmail(user.EmailAddress, Encode(model.Password));
                TempData[CommonConstants.Constants.SuccessMessage] = "Password changed successfully.";

                try
                {
                    SendEmail(user.EmailAddress, "Password changed successfully.");
                }
                catch (Exception ex)
                { }

            }
            else
            {
                if (!IsCurrentPass)
                {
                    TempData[CommonConstants.Constants.ErrorMessage] = "Current password doesnot match.";
                }
                else
                {
                    TempData[CommonConstants.Constants.ErrorMessage] = "Password and confirm password doesnot match.";
                }
            }

            return RedirectToAction("ChangePassword");
        }

        public ActionResult UserInfo()
        {
            var user = (User)Session[CommonConstants.Constants.User];

            var userDetails = userProvider.GetUserByEmail(user.EmailAddress);

            return View(MapUserToModel(userDetails));
        }

        public ActionResult UpdateUserInfo(RegisterSignin model)
        {
            try
            {
                var user = (User)Session[CommonConstants.Constants.User];
                model.EmailAddress = user.EmailAddress;
                userProvider.UpdateUserByEmail(MapUpdatedUserToDomain(model));

                TempData[CommonConstants.Constants.SuccessMessage] = "User updated successfully.";
            }
            catch (Exception ex)
            {
                TempData[CommonConstants.Constants.ErrorMessage] = "User doesnot updated successfully.";
            }

            return RedirectToAction("UserInfo");
        }

        #region Private


        private void SendEmail(string recipient, string message)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(recipient);
            mail.From = new MailAddress("myemail");
            mail.Subject = "Forget Password";
            mail.Body = message;// "send passsword in this body";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("myemail", "password");
            smtp.Send(mail);
        }

        private User MapUserToModel(DomainEntities.User user)
        {
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Role = user.Organisation != null ? user.Organisation.RoleName : string.Empty,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                OrganisationId = user.OrganisationId,
                Phone = user.Phone,
                PostalCode = user.PostalCode,
                State = user.State,
                Street = user.Street,
                UniqueId = user.UniqueId
            };
        }

        private DomainEntities.User MapUpdatedUserToDomain(RegisterSignin user)
        {
            return new DomainEntities.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                Phone = user.Phone,
                PostalCode = user.PostalCode,
                State = user.State,
                Street = user.Street,
                EmailAddress = user.EmailAddress
            };
        }

        private static string Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static string Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private DomainEntities.User MapUserToDomain(OnlineShop.Models.RegisterSignin user)
        {
            return new DomainEntities.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                Phone = user.Phone,
                PostalCode = user.PostalCode,
                State = user.State,
                Street = user.Street,
                CreationTime = DateTime.Now,
                IsActive = true,
                Password = Encode(user.Password),
                UniqueId = Guid.NewGuid().ToString()
            };
        }

        #endregion
    }
}