using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [AllowAnonymous]
    public class MapController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}