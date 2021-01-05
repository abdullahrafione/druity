using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.CommonConstants
{
    public static class Constants
    {
        #region Role
        public const string OwnerRole = "Owner";
        public const string AdminRole = "Admin";
        public const string BuyerRole = "Buyer";
        #endregion

        #region cookies names
        public const string cartcookieName = "cart";
        public const string cartStocktId = "S";
        public const string cartQuantiy = "Q";
        #endregion

        #region Session
        public const string User = "LoggedinUser";
        #endregion

        #region Error
        public const string ErrorMessage = "Error";
        public const string SuccessMessage = "Success";
        #endregion

        #region Common
        public static string Pound = "Rs";
        #endregion
    }
}