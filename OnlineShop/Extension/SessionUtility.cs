using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Entension
{
    public static class SessionUtility
    {
        public static User GetLogedInUser()
        {
            var user = (User)HttpContext.Current.Session[CommonConstants.Constants.User];
            return user;
        }

        public static MvcHtmlString GetCurrentUserName(this HtmlHelper html)
        {
            string displayName = string.Empty;

            var user = (User)HttpContext.Current.Session[CommonConstants.Constants.User];
            if (user != null)
            {
                displayName = user.FirstName + " " + user.LastName + " ";
            }

            return new MvcHtmlString(displayName);
        }
    }
}