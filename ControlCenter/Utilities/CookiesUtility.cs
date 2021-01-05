using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlCenter.Utilities
{
    public static class CookiesUtility
    {

        public static string Cookieindex(int index, HttpContextBase httpContextBase)
        {
            var context = httpContextBase.Request.Cookies["userInfo"].Value;
            context = HttpUtility.UrlDecode(context ?? string.Empty);
            string[] values = context.Split('&').Select(x => x.Trim()).ToArray();
            var cookievalue = GetCookieValues(values[index]);

            return cookievalue;
        }

        public static string GetCookieValues(string value)
        {
            string[] data = value.Split('=').Select(x => x.Trim()).ToArray();
            value = data[1];
            return value;
        }

        public static string GetLoggedInUserName(HttpContextBase httpContextBase)
        {
            string UserName = string.Empty;

            HttpCookie reqCookies = httpContextBase.Request.Cookies["userInfo"];

            if (reqCookies != null)
            {
                UserName = reqCookies["UserName"].ToString();
                return UserName;
            }
            return null;
        }

    }
}