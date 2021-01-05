using System.Web.Mvc;
using DataAccess.Providers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Web;
using System.IO;

namespace ControlCenter.Controllers
{
    public class ProductController : Controller
    {
        #region Constructors

        private readonly ProductProvider productProvider;

        public ProductController()
        {
            productProvider = new ProductProvider();
        }

        #endregion

        #region Product
        public ActionResult Index()
        {
            var products = productProvider.GetAllProducts();
            return View(MaptoModel(products));
        }

        public ActionResult AddProduct()
        {
            List<Models.Category> list = new List<Models.Category>();
            var categories = MapCategoryToModel(productProvider.GetCategories());
            TempData["categories"] = categories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Models.Product product)
        {
            int addedproductId = productProvider.AddProduct(MaptoDomain(product));

            return RedirectToAction("EditStockByProductId","Product", new { productId = addedproductId });
        }

        public ActionResult EditStockByProductId(int productId)
        {
            var product = productProvider.GetProductByProductId(productId);

            return View(Mapping(product));
        }

        [HttpPost]
        public ActionResult UpdateStock(Models.Product product)
        {
            int addedproductId = productProvider.UpdateStock(MaptoProductStock(product));

            return RedirectToAction("ProductImage","Product", new {productId = addedproductId});
        }

        public ActionResult EditProduct (int productId)
        {
            var product = productProvider.GetProductByProductId(productId);

            return View(MapNew(product));
        }

        [HttpPost]
        public ActionResult EditProduct(Models.Product product)
        {
            productProvider.UpdateProduct(MapedtoDomain(product));

            return RedirectToAction("Index");
        }

        #endregion

        #region ProductImages
        public ActionResult ProductImage(int productId)
        {
            TempData["ProductId"] = productId;

            return View();
        }

        [HttpPost]
        public ActionResult SaveImage(DataAccess.Domain.Image image)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        string uniqueness = Guid.NewGuid().ToString();
                        fname = Path.GetFileName(file.FileName);

                        var path = Path.Combine(Server.MapPath("~/Uploads/ProductImages"), uniqueness + fname);
                        file.SaveAs(path);

                        image.Url = System.Configuration.ConfigurationManager.AppSettings["DashboardUrl"] + "/Uploads/ProductImages/" + uniqueness + fname;
                        image.Name = uniqueness + fname;

                    }

                    productProvider.AddProductImages(image);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return RedirectToAction("Index");
        }

        #endregion
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

        private DataAccess.Domain.Product MaptoDomain (Models.Product product)
        {
            return new DataAccess.Domain.Product
            {
                CategoryId = product.CategoryId,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Detail = product.Detail,
                IsActive = true,
                IsDeleted = false,
                CreationTime = DateTime.Now,
                GenderTagId = 1,
                OrganisationId = 1
            };
        }

        private Models.Product Mapping (DataAccess.Domain.Product product)
        {
            return new Models.Product
            {
                Name = product.Name,
                productId = product.Id
            };
        }

        private DataAccess.Domain.ProductStock MaptoProductStock (Models.Product product)
        {
            return new DataAccess.Domain.ProductStock
            {
                ColourId = 1,
                IsActive = true,
                IsDeleted = false,
                CreationTime = DateTime.Now,
                SizeID = 1,
                OnSale = false,
                IsFeatured = false,
                TopRated = false,
                CurrencySymbol = "Rs ",

                CurrentPrice = product.CurrentPrice,
                OldPrice = product.OldPrice,
                StockCount = product.StockCount,
                ProductId = product.productId
            };
        }

        private List<Models.Product> MaptoModel (List<DataAccess.Domain.Product> products)
        {
            List<Models.Product> mapped = new List<Models.Product>();
            products.ForEach(x =>
            {
                mapped.Add(new Models.Product
                {
                    CategoryId = x.CategoryId,
                    CreatedOn = x.CreationTime,
                    Name = x.Name,
                    Detail = x.Detail,
                    productId = x.Id
                });
            });

            return mapped;
        }

        private Models.Product MapNew (DataAccess.Domain.Product product)
        {
            return new Models.Product
            {
                Name = product.Name,
                Detail = product.Detail,
                ShortDescription = product.ShortDescription,
                productId = product.Id
            };
        }

        private DataAccess.Domain.Product MapedtoDomain (Models.Product product)
        {
            return new DataAccess.Domain.Product
            {
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Detail = product.Detail,
                Id = product.productId
            };
        }

        #endregion
    }
}