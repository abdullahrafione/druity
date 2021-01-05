using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Providers;

namespace ControlCenter.Controllers
{
    public class UserController : Controller
    {
        #region Constructors

        private readonly UserProvider userProvider;

        public UserController()
        {
            userProvider = new UserProvider();
        }
        #endregion

        public ActionResult MarketPlaceUsers()
        {
            var users = userProvider.GetUsers();

            return View(MaptoModel(users));
        }

        #region Private

        private List<Models.UserModel> MaptoModel(List<DataAccess.Domain.User> users)
        {
            List<Models.UserModel> mapped = new List<Models.UserModel>();

            users.ForEach(x =>
            {
                mapped.Add(new Models.UserModel
                {
                    EmailAddress = x.EmailAddress,
                    Address = x.Address,
                    City = x.City,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNo = x.Phone
                });
            });

            return mapped;
        }

        #endregion
    }
}