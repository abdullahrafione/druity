using DataAccess.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlCenter.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        #region Props
        private readonly ProductProvider productProvider;
        private readonly CommonProvider commonProvider;
        public CommonController()
        {
            productProvider = new ProductProvider();
            commonProvider = new CommonProvider();
        }
        #endregion
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Category()
        {
            List<Models.Category> list = new List<Models.Category>();
            var categories = MapCategoryToModel(productProvider.GetMainCategories());
            TempData["categories"] = categories.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Category(ControlCenter.Models.Category category)
        {
            commonProvider.AddCategory(Map(category));

            return RedirectToAction("Category");
        }

        [HttpPost]
        public ActionResult AddMainCategory(Models.Category category)
        {
            commonProvider.AddCategory(MapMainCategory(category));

            return RedirectToAction("Category");
        }

        public ActionResult UpdateScrapping()
        {

            return View();
        }

        [HttpPost]
        public ActionResult UpdateScrapping(Models.UserModel model)
        {
            UpdateScrapping(model.FirstName, model.LastName);

            return RedirectToAction("UpdateScrapping");
        }

        public ActionResult DruityBanners()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult DruityBanners()
        //{
        //    return View();
        //}

        #region Private
        private List<Models.Category> MapCategoryToModel(List<DataAccess.Domain.Category> categories)
        {
            List<Models.Category> mappedList = new List<Models.Category>();
            categories.ForEach(x =>
            {
                mappedList.Add(new Models.Category
                {
                    CategoryId = x.Id,
                    CategoryName = x.Name,
                    ParentCategoryId = x.ParentCategoryId
                });
            });
            return mappedList;
        }
        
        private DataAccess.Domain.Category Map(Models.Category category)
        {
            return new DataAccess.Domain.Category
            {
                Name = category.CategoryName,
                IsActive = true,
                IsDeleted = false,
                ParentCategoryId = category.ParentCategoryId,
                DisplayOrder = category.DisplayOrder,
                CreationTime = DateTime.Now,
                Tag = category.CategoryName
            };
        }
        private DataAccess.Domain.Category MapMainCategory(Models.Category category)
        {
            return new DataAccess.Domain.Category
            {
                Name = category.CategoryName,
                IsActive = true,
                IsDeleted = false,
                ParentCategoryId = 0,
                DisplayOrder = category.DisplayOrder,
                CreationTime = DateTime.Now,
                Tag = category.CategoryName
            };
        }

        protected void UpdateScrapping(string userName, string apiKey)
        {
            Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");

            if (objAppsettings != null)
            {
                objAppsettings.Settings["ScrapBot_UserName"].Value = userName;
                objAppsettings.Settings["ScrapBot_ApiKey"].Value = apiKey;
                objConfig.Save();
            }
        }

        #endregion
    }
}