using OnlineShop.Auth;
using OnlineShop.DAL;
using OnlineShop.Entension;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [Authorize]
    [AuthorizationFilter(CommonConstants.Constants.AdminRole)]
    public class AdminController : Controller
    {
        private static string message;
        private readonly ProductProvider productProvider;
        private readonly ImageProvider imageProvider;
        private readonly OrderProvider orderProvider;
        private readonly UserProvider userProvider;
        public AdminController()
        {
            productProvider = new ProductProvider();
            imageProvider = new ImageProvider();
            orderProvider = new OrderProvider();
            userProvider = new UserProvider();
        }
        // GET: Admin
        public ActionResult Dashboard()
        {
            List<User> users = MapUserToModel(userProvider.GetUsers());

            return View(users);
        }

        #region LandingPage

        public ActionResult UploadBanner()
        {
            var bannerImages = MaptoGeneralimages(imageProvider.GetGeneralImages());

            return View(bannerImages);
        }

        public ActionResult UploadImageBanner()
        {
            return View();
        }

        public PartialViewResult UploadBannerImage(GeneralImages image)
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

                        var path = Path.Combine(Server.MapPath("~/Uploads"), uniqueness + fname);
                        file.SaveAs(path);

                        image.Url = System.Configuration.ConfigurationManager.AppSettings["WebUrl"] + "/Uploads/" + uniqueness + fname;
                        image.Name = uniqueness + fname;
                    }
                    //var allImages = imageProvider.SaveImage(image);
                    //return PartialView("~/Views/Admin/_ImagesList.cshtml", allImages);
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return PartialView();
        }

        public ActionResult ConfigureBanner(int image)
        {

            return RedirectToAction("UploadBanner");
        }

        #endregion

        #region Dashboard landing page
        public PartialViewResult GetStats()
        {
            //Oerder last 7 days
            //orderswith status open
            //total Users
            //contact Messages unread
            //bulk order request last 7 days
            //CancelledOrders
            return PartialView();
        }
        #endregion

        #region Orders
        public ActionResult Orders(OrderHistory orderHistory)
        {

            return View(orderHistory);
        }

        public ActionResult ApplyFilter(OrderHistory orderHistory)
        {
            orderHistory.IsFilter = true;
            return RedirectToAction("Orders", orderHistory);
        }

        [HttpPost]
        public PartialViewResult LoadOrders(string filterValues)
        {
            string[] values = filterValues.Split('|');

            OrderHistory orderHistory = new OrderHistory();
            orderHistory.StartDate = Convert.ToDateTime(values[0]);
            orderHistory.EndDate = Convert.ToDateTime(values[1]);
            orderHistory.SearchText = values[2];
            orderHistory.IsFilter = Convert.ToBoolean(values[3]);
            var status = orderProvider.GetOrderStatus();
            var orders = orderProvider.GetOrdersAdmin(orderHistory);
            orders.ForEach(x =>
            {
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;

            });
            if (orders.Any())
            {
                orders.FirstOrDefault().OrderStatuses = status;
            }
            return PartialView("_Orders",orders);
        }

        public PartialViewResult ChangeStatus(int statusId, int orderDetailid)
        {
            orderProvider.ChangeStatus(statusId, orderDetailid);
            return null;
        }

        public PartialViewResult LoadOrders(Models.OrderHistory orderHistory)
        {
            var status = orderProvider.GetOrderStatus();
            var orders = orderProvider.GetOrdersAdmin(orderHistory);

            orders.ForEach(x =>
            {
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;

            });
            if (orders.Any())
            {
                orders.FirstOrDefault().OrderStatuses = status;
            }

            return PartialView("~/Views/Admin/_Orders.cshtml", orders);
        }
        #endregion

        #region Product
        public ActionResult AddProduct()
        {
            List<Category> list = new List<Category>();
            var categories = productProvider.GetCategories();
            categories = categories.Where(x => x.ParentCategoryId == 0).ToList();

            return View(categories);
        }

        public PartialViewResult Getgendertags()
        {
            var GenderTags = productProvider.GetGenderTags();
            return PartialView("_GetGendersList", GenderTags);
        }

        public PartialViewResult GetChildCategories(int id)
        {
            var categories = productProvider.GetCategories();
            categories = categories.Where(x => x.ParentCategoryId == id).ToList();

            return PartialView("_GetChildCategories", categories);
        }

        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            try
            {
                productProvider.AddProduct(MapToDomain(product));
                return RedirectToAction("Products", "admin", new { currentPage = 1 });
            }
            catch (Exception ex)
            {
                List<Category> list = new List<Category>();
                var categories = productProvider.GetCategories();
                categories = categories.Where(x => x.ParentCategoryId == 0).ToList();
                TempData["Error"] = "Please select sub category.";
                return View(categories);
               
            }
          
        }

        [HttpPost]
        public ActionResult UpdateProduct(Product product)
        {
            product.OrganisationId = GetLogedInUser().OrganisationId;
            productProvider.UpdateProduct(product);
            return null;
        }

        [HttpPost]
        public ActionResult MarkProductActive(int productId)
        {
            productProvider.MarkProductActive(productId);
            return null;
        }

        [HttpPost]
        public ActionResult MarkProduct_DeActive(int productId)
        {
            productProvider.MarkProduct_DeActive(productId);
            return null;
        }

        public ActionResult Products(int currentPage)
        {
            var products = productProvider.GetProducts(currentPage);
            products.Products.ForEach(x => 
            {
                x.Image.Add(imageProvider.GetImages(x.ProductId).FirstOrDefault());
            });
            return View(products);
        }

        public ActionResult ConfigureProduct(int productId)
        {
            var response = productProvider.ConfigureProduct(GetLogedInUser().OrganisationId, productId);
            if(response!=null)
            {
                response.Images = imageProvider.GetImages(response.Product.ProductId);
            }
           
            return View(response);
        }

        [HttpPost]
        public ActionResult SaveProductStock(ProductStock stock)
        {
            try
            {
                DomainEntities.ProductStock productStock = new DomainEntities.ProductStock
                {
                    ColourId = stock.ColourId,
                    CurrentPrice = stock.CurrentPrice,
                    IsFeatured = stock.IsFeatured,
                    IsActive = true,
                    OnSale = stock.OnSale,
                    ProductId = stock.ProductId,
                    OldPrice = stock.OldPrice,
                    SizeID = stock.SizeID,
                    TopRated = stock.TopRated,
                    StockCount = stock.StockCount,
                    CurrencySymbol = CommonConstants.Constants.Pound
                };
                
                productProvider.AddProductStock(productStock);
                TempData[CommonConstants.Constants.SuccessMessage] = "Stock update successfully";
                return RedirectToAction("ConfigureProduct", "admin", new { productId = stock.ProductId });
            }
            catch (Exception ex)
            {
                TempData[CommonConstants.Constants.ErrorMessage] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateStock(ProductStock stock)
        {
            productProvider.UpdateStock(stock);
            return RedirectToAction("ConfigureProduct","Admin",new { productId=stock.ProductId});
        }

        public ActionResult DeleteStock(ProductStock stock)
        {
            try
            {
                productProvider.DeleteStock(GetLogedInUser().OrganisationId, stock.ProductStockId);
                return RedirectToAction("ConfigureProduct", "Admin", new { productId = stock.ProductId });
            }
            catch (Exception ex)
            {
                TempData[CommonConstants.Constants.SuccessMessage]= ex.Message;
                return RedirectToAction("ConfigureProduct", "Admin", new { productId = stock.ProductId });
            }
          
        }

        #endregion

        #region Image
        [HttpPost]
        public PartialViewResult UploadImage(Image image)
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

                        var path = Path.Combine(Server.MapPath("~/Uploads"), uniqueness + fname);
                        file.SaveAs(path);

                        image.Url = System.Configuration.ConfigurationManager.AppSettings["WebUrl"]+ "/Uploads/" + uniqueness + fname;
                        image.Name = uniqueness + fname;

                        //System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(file.InputStream);

                        //// height Width and Colour will be dynamic ..for now we use static  
                        //System.Drawing.Image resizedImage;
                        //resizedImage = Resize(bmpPostedImage, 800, 800, true);

                        //System.Drawing.Image canvasImage;
                        //canvasImage = PutOnCanvas(resizedImage, 800, 800, System.Drawing.Color.White);

                        ////postedFile.SaveAs(filePath); // commented the Old one ..

                        //SaveJpeg(path, canvasImage);

                    }
                   var allImages = imageProvider.SaveImage(image);
                    return PartialView("~/Views/Admin/_ImagesList.cshtml",allImages);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult DeleteImage(int imageId, int productId)
        {
            try
            {
                var imageName =imageProvider.DeleteImageAndReturnName(imageId);
                var path = Path.Combine(Server.MapPath("~/Uploads"));
                if (System.IO.File.Exists(Path.Combine(path, imageName)))
                {
                    System.IO.File.Delete(Path.Combine(path, imageName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var allimages = imageProvider.GetImages(productId);
            return PartialView("~/Views/Admin/_ImagesList.cshtml", allimages);
        }
        #endregion

        #region Private

        private List<Models.GeneralImages> MaptoGeneralimages (List<DomainEntities.GeneralImages> images)
        {
            List<Models.GeneralImages> mapped = new List<GeneralImages>();
            images.ForEach(x =>
            {
                mapped.Add(new GeneralImages
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.Url
                });
            });

            return mapped;
        }
        private OnlineShop.DomainEntities.Product MapToDomain(Product product)
        {
            return new DomainEntities.Product
            {
                CategoryId = product.CategoryId,
                Detail = product.Detail,
                ShortDescription=product.ShortDescription,
                Name = product.Name,
                GenderTagId=product.GenderTagId,
                OrganisationId = GetLogedInUser().OrganisationId
            };
        }

        private User GetLogedInUser()
        {
            var user = (User)Session[CommonConstants.Constants.User];
            return user;
        }

        private static System.Drawing.Image Resize(System.Drawing.Image image, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {
            if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;

            var newHeight = image.Height * newWidth / image.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead  
                newWidth = image.Width * maxHeight / image.Height;
                newHeight = maxHeight;
            }

            var res = new System.Drawing.Bitmap(newWidth, newHeight);

            using (var graphic = System.Drawing.Graphics.FromImage(res))
            {
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return res;
        }

        private static System.Drawing.Image PutOnCanvas(System.Drawing.Image image, int width, int height, System.Drawing.Color canvasColor)
        {
            var res = new System.Drawing.Bitmap(width, height);
            using (var g = System.Drawing.Graphics.FromImage(res))
            {
                g.Clear(canvasColor);
                var x = (width - image.Width) / 2;
                var y = (height - image.Height) / 2;
                g.DrawImageUnscaled(image, x, y, image.Width, image.Height);
            }

            return res;
        }

        private static void SaveJpeg(string path, System.Drawing.Image img)
        {
            var qualityParam = new System.Drawing.Imaging.EncoderParameter(Encoder.Quality, 100L);
            var jpegCodec = GetEncoderInfo("image/jpeg");

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, jpegCodec, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == mimeType);
        }

        private List<User> MapUserToModel(List<DomainEntities.User> users)
        {
            List<User> mappedList = new List<User>();

            if (users.Any())
            {
                users.ForEach(u => {
                    User map = new User();
                    map.FirstName = u.FirstName;
                    map.LastName = u.LastName;
                    map.EmailAddress = u.EmailAddress;
                    map.Role = u.Organisation.RoleName;
                    map.Address = u.Address;
                    map.City = u.City;
                    map.Country = u.Country;
                    map.Phone = u.Phone;
                    map.PostalCode = u.PostalCode;
                    map.State = u.State;
                    map.Street = u.Street;

                    mappedList.Add(map);
                });
            }

            return mappedList;
        }
        #endregion
    }
}