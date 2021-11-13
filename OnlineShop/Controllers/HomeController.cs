using OnlineShop.DAL;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ProductProvider productProvider;
        private readonly ImageProvider imageProvider;

        public HomeController()
        {
            imageProvider = new ImageProvider();
            productProvider = new ProductProvider();
        }

        public ActionResult Index()
        {
            LandingPage landingPage = new LandingPage();

            var parentcategories = productProvider.GetCategories().Where(x => x.ParentCategoryId == 0).ToList();
            parentcategories.Add(new Models.Category { CategoryId = -1, CategoryName = "All", ParentCategoryId = -1 });
            landingPage.parentCategories = parentcategories.OrderBy(x => x.CategoryName).ToList();

            landingPage.products = productProvider.GetProductsForLandingPage();
            landingPage.products.Products.ForEach(x => { x.Image = imageProvider.GetImages(x.ProductId); });

            return View(landingPage);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Faqs()
        {
            return View();
        }
    }
}