using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Auth
{
    public class AuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public static string myrole = string.Empty;

        public AuthorizationFilter(string roles) : base()
        {
            myrole = roles;
            Roles = string.Join(",", roles);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                // Don't check for authorization as AllowAnonymous filter is applied to the action or controller  
                return;
            }

            if (HttpContext.Current.Session[CommonConstants.Constants.User] != null)
            {
                var user = (User)HttpContext.Current.Session[CommonConstants.Constants.User];
                if (user.Role != myrole)
                {
                    filterContext.Result = new RedirectResult("~/error/notfound");
                }
            }

            // Check for authorization  
            if (HttpContext.Current.Session[CommonConstants.Constants.User] == null)
            {
                filterContext.Result = new RedirectResult("~/account/signin");
            }
        }
    }
}