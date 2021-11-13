using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataAccess.Providers;

namespace ControlCenter.Controllers
{
    public class AccountController : Controller
    {
        #region Constructors

        private readonly UserProvider userProvider;

        public AccountController()
        {
            userProvider = new UserProvider();
        }

        #endregion

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(Models.UserModel model, string returnUrl)
        {
            // Lets first check if the Model is valid or not
            if (ModelState.IsValid)
            {

                string username = model.EmailAddress;
                string password = model.Password;

                // Now if our password was enctypted or hashed we would have done the
                // same operation on the user entered password here, But for now
                // since the password is in plain text lets just authenticate directly

                var user = userProvider.Authenticate(model.EmailAddress, model.Password);
                bool exist = false;
                if (user.EmailAddress == username && user.Password == password)
                {
                    exist = true;
                }

                // User found in the database
                if (exist)
                {
                    FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        HttpCookie userInfo = new HttpCookie("userInfo");
                        userInfo["UserName"] = user.FirstName + " " + user.LastName;
                        userInfo["Email"] = user.EmailAddress;
                        userInfo["Role"] = "Admin";
                        userInfo.Expires.Add(new TimeSpan(2, 0, 0));
                        Response.Cookies.Add(userInfo);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        //[HttpPost]
        //public ActionResult Login(DataAccess.Domain.User model, string returnUrl)
        //{

        //    // Lets first check if the Model is valid or not
        //    //if (ModelState.IsValid)
        //    //{
        //        string username = model.EmailAddress;
        //        string password = model.Password;

        //        // Now if our password was enctypted or hashed we would have done the
        //        // same operation on the user entered password here, But for now
        //        // since the password is in plain text lets just authenticate directly

        //        var user = userProvider.Authenticate(username, password);

        //        bool exist = false;
        //        if (user.EmailAddress == username && user.Password == password)
        //        {
        //            exist = true;
        //        }

        //        // User found in the database
        //        if (exist)
        //        {
        //            FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
        //            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
        //                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
        //            {
        //                return Redirect(returnUrl);
        //            }
        //            else
        //            {
        //                HttpCookie userInfo = new HttpCookie("userInfo");
        //                userInfo["UserName"] = user.FirstName + " " + user.LastName;
        //                userInfo["Email"] = user.EmailAddress;
        //                //userInfo["Role"] = user.UserRoles.FirstOrDefault().Role.Name.ToString();
        //                userInfo.Expires.Add(new TimeSpan(2, 0, 0));
        //                Response.Cookies.Add(userInfo);

        //                return RedirectToAction("Index", "Home");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "The user name or password provided is incorrect.");
        //        }


        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "account");
        }

    }
}