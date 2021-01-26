using System.Web.Mvc;
using DataAccess.Providers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;

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

            return RedirectToAction("EditStockByProductId", "Product", new { productId = addedproductId });
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

            return RedirectToAction("ProductImage", "Product", new { productId = addedproductId });
        }

        public ActionResult EditProduct(int productId)
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

        #region Scarpping
        public ActionResult GetProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProduct(Models.ScrapProductModel scrapProductModel)
        {
            List<Models.Category> list = new List<Models.Category>();
            var categories = MapCategoryToModel(productProvider.GetCategories());
            TempData["categories"] = categories.ToList();


            var username = ConfigurationManager.AppSettings["ScrapBot_UserName"];
            var apiKey = ConfigurationManager.AppSettings["ScrapBot_ApiKey"];

            var byteArray = Encoding.ASCII.GetBytes(username + ":" + apiKey);
            var auth = Convert.ToBase64String(byteArray);
            var apiEndPoint = ConfigurationManager.AppSettings["ScrapBot_EndPoint"];

            var values = new
            {
                url = scrapProductModel.URL,
                options = new
                {
                    useChrome = false,
                    premiumProxy = false,
                    waitForNetworkRequests = false
                }
            };

            var json = JsonConvert.SerializeObject(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var response = httpClient.PostAsync(apiEndPoint, content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<Models.ScrapResponse.Root>(responseString);
                var model = ConvertResponse(result);
                return View("~/Views/Product/ScrapResponse.cshtml", model);
            }
            else

                return View("Error");

        }

        [HttpPost]
        public ActionResult SaveProduct(Models.Product product)
        {
            int productId = productProvider.AddProduct(MaptoDomain(product));
            var productStock = UpdateStock(product, productId);
            productProvider.UpdateStock(productStock);
            var domainImage = SaveImage(product, productId);
            productProvider.AddProductImages(domainImage);

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

        private DataAccess.Domain.Product MaptoDomain(Models.Product product)
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

        private DataAccess.Domain.ProductStock UpdateStock(Models.Product product, int prodactId)
        {
            return new DataAccess.Domain.ProductStock
            {
                StockCount = 10,
                IsFeatured = false,
                ColourId = 1,
                IsActive = true,
                IsDeleted = false,
                CreationTime = DateTime.Now,
                SizeID = 1,
                OnSale = false,
                TopRated = false,
                CurrencySymbol = "Rs ",

                CurrentPrice = product.CurrentPrice,
                OldPrice = product.OldPrice,
                ProductId = prodactId
            };
        }

        private DataAccess.Domain.Image SaveImage(Models.Product product, int prodactId)
        {
            return new DataAccess.Domain.Image
            {
                ProductId = prodactId,
                DisplayOrder = 0,
                Name = "Scrapping-ThirdParty",
                CreationTime = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                IsPrimary = false,
                IsSizeGuide = false,
                Url = product.ImageUrl
            };
        }

        private Models.Product Mapping(DataAccess.Domain.Product product)
        {
            return new Models.Product
            {
                Name = product.Name,
                productId = product.Id
            };
        }

        private DataAccess.Domain.ProductStock MaptoProductStock(Models.Product product)
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

        private List<Models.Product> MaptoModel(List<DataAccess.Domain.Product> products)
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

        private Models.Product MapNew(DataAccess.Domain.Product product)
        {
            return new Models.Product
            {
                Name = product.Name,
                Detail = product.Detail,
                ShortDescription = product.ShortDescription,
                productId = product.Id
            };
        }

        private DataAccess.Domain.Product MapedtoDomain(Models.Product product)
        {
            return new DataAccess.Domain.Product
            {
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Detail = product.Detail,
                Id = product.productId
            };
        }

        private Models.Product ConvertResponse(Models.ScrapResponse.Root obj)
        {
            return new Models.Product
            {
                Name = obj.data.title,
                ShortDescription = obj.data.description,
                CurrentPrice = obj.data.price,
                ImageUrl = obj.data.image
            };
        }

        #endregion
    }
}