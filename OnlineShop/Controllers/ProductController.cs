using OnlineShop.DAL;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [AllowAnonymous]
    public class ProductController : Controller
    {
        private readonly ProductProvider productProvider;
        private readonly ImageProvider imageProvider;
        public ProductController()
        {
            imageProvider = new ImageProvider();
            productProvider = new ProductProvider();
        }
        // GET: Product Categoriesfilter
        public PartialViewResult Categories()
        {
            var categories = productProvider.GetCategories();
            var parentcategories = categories.Where(x => x.ParentCategoryId == 0).ToList();

            List<ParentChildCategory> mappedList = new List<ParentChildCategory>();
            parentcategories.ForEach(c =>
            {
                ParentChildCategory model = new ParentChildCategory();
                model.ParentCategoryId = c.CategoryId;
                model.ParentCategoryName = c.CategoryName;
                model.ChildCategories = categories.Where(x => x.ParentCategoryId == c.CategoryId).ToList();
                mappedList.Add(model);
            });
            mappedList.FirstOrDefault().ParentCategories = parentcategories;
            return PartialView("~/Views/Shared/_StickyCategoriesMenu.cshtml", mappedList);
        }

        public PartialViewResult GetCategoriesForMobileView()
        {
            var categories = productProvider.GetCategories();
            var parentcategories = categories.Where(x => x.ParentCategoryId == 0).ToList();

            List<ParentChildCategory> mappedList = new List<ParentChildCategory>();
            parentcategories.ForEach(c =>
            {
                ParentChildCategory model = new ParentChildCategory();
                model.ParentCategoryId = c.CategoryId;
                model.ParentCategoryName = c.CategoryName;
                model.ChildCategories = categories.Where(x => x.ParentCategoryId == c.CategoryId).ToList();
                mappedList.Add(model);
            });
            mappedList.FirstOrDefault().ParentCategories = parentcategories;

            return PartialView("~/Views/Shared/_MobileViewCategories.cshtml", mappedList);
        }

        [HttpPost]
        public PartialViewResult GetCategoriesSizeColoursFilter(CategorySelecter selecter)
        {
            CategoriesSizeColoursFilter model = new CategoriesSizeColoursFilter();
            if (selecter.ParentSelectedCategoryId > 0 && selecter.IsMainSelected)
            {
                model = productProvider.GetCategoriesSizeColoursFilter(selecter.ParentSelectedCategoryId, true);
                model.ParentSelectedCategoryId = selecter.ParentSelectedCategoryId;
                model.IsMainSelected = true;
            }
            else if (selecter.ChildSelectedCategory > 0 && selecter.IsChildSelected)
            {
                model = productProvider.GetCategoriesSizeColoursFilter();
                model.ChildSelectedCategory = selecter.ChildSelectedCategory;
                model.IsChildSelected = true;
            }
            else
            {
                model = productProvider.GetCategoriesSizeColoursFilter();
            }

            return PartialView("~/Views/Product/_ShopSideFilter.cshtml", model);
        }

        //Experiment Search Text
        public JsonResult GetCustomers(string term)
       {
            DbFactory.ShopDbContext context = new DbFactory.ShopDbContext();
           
            List<string> Products =  context.Product.Where(x => x.Name.Contains(term))
                .Select(y => y.Name).ToList();
            
            return Json(Products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Shop(string searchText,int category = default(int), bool main = default(bool))
        {
            int currentpage = 1;
            if(searchText != null && searchText != "")
            {
                var shopResponse = productProvider.GetProductsForShop(currentpage, new List<int>(),default(int), default(int), searchText);
                shopResponse.Products.ForEach(x => { x.Image = imageProvider.GetImages(x.ProductId); });
                shopResponse.ParentSelectedCategoryId = category; shopResponse.IsMainSelected = true;

                return View(shopResponse);
            }

            if (main)
            {
                var shopResponse = productProvider.GetProductsForShop(currentpage,new List<int>() ,category);
                shopResponse.Products.ForEach(x => { x.Image = imageProvider.GetImages(x.ProductId); });
                shopResponse.ParentSelectedCategoryId = category; shopResponse.IsMainSelected = true;

                return View(shopResponse);
            }
            else if (category > 0 && !main)
            {
                var shopResponse = productProvider.GetProductsForShop(currentpage, new List<int>(), 0, category);
                shopResponse.Products.ForEach(x => { x.Image = imageProvider.GetImages(x.ProductId); });
                shopResponse.ChildSelectedCategory = category; shopResponse.IsChildSelected = true;
                return View(shopResponse);
            }
            else
            {
                var shopResponse = productProvider.GetProductsForShop(currentpage, new List<int>());
                shopResponse.Products.ForEach(x => { x.Image = imageProvider.GetImages(x.ProductId); });

                return View(shopResponse);
            }

            
        }

        public ActionResult Detail(int id)
        {
            var products = productProvider.GetbyId(id);
            products.Products.ForEach(x =>
            {
                x.Image = imageProvider.GetImages(x.ProductId);
            });
            return View(products.Products.FirstOrDefault());
        }

        public PartialViewResult GetSizesColours(int productId)
        {
            var response = productProvider.GetColorsSizes(productId);
            response.Sizes = response.Sizes.GroupBy(x => x.Name).Select(g => g.First()).ToList();
            response.Colours = response.Colours.GroupBy(x => x.Name).Select(g => g.First()).ToList();
            return PartialView("~/Views/Product/_DetailSizeColorActions.cshtml", response);
        }

        public PartialViewResult Getgendertags()
        {
            var GenderTags = productProvider.GetGenderTags();
            return PartialView("_GenderTagFilter", GenderTags);
        }

        public PartialViewResult GetColor(int sizeid,int productid)
        {
            var colors = productProvider.GetColors(sizeid, productid);
            return PartialView("~/Views/Shared/_Detailcolors.cshtml", colors);
        }

        [HttpPost]
        public PartialViewResult Filter(FilterModel model)
        {
            string[] parentcategoriesBreak = model.ParentCategories.Split('#');
            string[] childcategoriesBreak = model.ChildCategories.Split('#');
            string[] sizesBreak = model.Sizes.Split('#');
            //string[] coloursBreak = model.Colours.Split('#');

            var criteria = BuildFilterCriteria(parentcategoriesBreak, childcategoriesBreak, sizesBreak, model.SearchText, model.GenderTagId, model.CurrentPage);
            var filterResponse = productProvider.Filter(criteria);

            filterResponse.Products.ForEach(x =>
            {
                x.Image = imageProvider.GetImages(x.ProductId);
            });
            var rtes = filterResponse;
            return PartialView("~/Views/Product/_ShopGridView.cshtml", filterResponse);
        }

        #region Private
        private FilterCriteria BuildFilterCriteria(string[] parentcategories, string[] Childcategories, string[] sizes, string searchText, int genderTagId, int currentPage = 1)
        {
            FilterCriteria criteria = new FilterCriteria();
            foreach (var item in parentcategories)
            {
                var cat = item.Split(':');
                if ((cat[0] != string.Empty) && Convert.ToInt16(cat[1]) > 0)
                {
                    criteria.ParentCategoryIds.Add(Convert.ToInt16(cat[1]));
                }
            }

            foreach (var item in Childcategories)
            {
                var cat = item.Split(':');
                if ((cat[0] != string.Empty) && Convert.ToInt16(cat[1]) > 0)
                {
                    criteria.ParentCategoryIds.Add(Convert.ToInt16(cat[1]));
                }
            }

            foreach (var item in sizes)
            {
                var size = item.Split(':');
                if (size[0] != string.Empty && Convert.ToInt16(size[1]) > 0)
                {
                    criteria.SizeIds.Add(Convert.ToInt16(size[1]));
                }
            }

            //foreach (var item in colours)
            //{
            //    var colour = item.Split(':');
            //    if (colour[0] != string.Empty && Convert.ToInt16(colour[1]) > 0)
            //    {
            //        criteria.ColourIds.Add(Convert.ToInt16(colour[1]));
            //    }
            //}

            criteria.SearchText = searchText;
            criteria.CurrentPage = currentPage;
            criteria.GenderTagId.Add(genderTagId);

            return criteria;
        }
        
        #endregion
    }
}