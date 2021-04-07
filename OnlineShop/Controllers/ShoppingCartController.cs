using OnlineShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [AllowAnonymous]
    public class ShoppingCartController : Controller
    {
        private readonly ProductProvider productProvider;
        private readonly ImageProvider imageProvider;
        public ShoppingCartController()
        {
            productProvider = new ProductProvider();
            imageProvider = new ImageProvider();
        }

        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult GetItemsInCart()
        {
            var cart = this.GetCartItems();
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x =>
            {
                x.Quantity = cart.Where(c => c.ProductStockId == x.ProductStockId).ToList().Sum(v => v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });
            return PartialView("~/Views/ShoppingCart/_GetItemsInCart.cshtml", productsInCart);
        }

        public PartialViewResult GetItemsPlaceGuestOrder()
        {
            var cart = this.GetCartItems();
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x =>
            {
                x.Quantity = cart.Where(c => c.ProductStockId == x.ProductStockId).ToList().Sum(v => v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });
            return PartialView("~/Views/Order/_PlaceOrderForGuest.cshtml", productsInCart);
        }

        public PartialViewResult GetItemsPlaceOrder()
        {
            var cart = this.GetCartItems();
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x =>
            {
                x.Quantity = cart.Where(c => c.ProductStockId == x.ProductStockId).ToList().Sum(v => v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });
            return PartialView("~/Views/Order/_PlaceOrder.cshtml", productsInCart);
        }

        public ActionResult DeleteFromMainCart(int productStockId)
        {
            HttpCookie reqCookies = Request.Cookies[CommonConstants.Constants.cartcookieName];
            string stockIds = string.Empty;
            stockIds = reqCookies[CommonConstants.Constants.cartStocktId].ToString();

            string stringTobeReplaced = productStockId + ",1";
            string updatedString = stockIds.Replace(stringTobeReplaced, string.Empty);

            HttpCookie cartinfo = new HttpCookie(CommonConstants.Constants.cartcookieName);
            cartinfo[CommonConstants.Constants.cartStocktId] = updatedString;

            cartinfo.Expires.Add(new TimeSpan(12, 0, 0));

            Response.Cookies.Add(cartinfo);

            return RedirectToAction("cart");
        }

        [HttpGet]
        public PartialViewResult DeleteFromCart(int productStockId)
        {
            HttpCookie reqCookies = Request.Cookies[CommonConstants.Constants.cartcookieName];
            string stockIds = string.Empty;
            stockIds = reqCookies[CommonConstants.Constants.cartStocktId].ToString();

            string stringTobeReplaced = productStockId + ",1";
            string updatedString = stockIds.Replace(stringTobeReplaced, string.Empty);

            HttpCookie cartinfo = new HttpCookie(CommonConstants.Constants.cartcookieName);
            cartinfo[CommonConstants.Constants.cartStocktId] = updatedString;

            cartinfo.Expires.Add(new TimeSpan(12, 0, 0));

            Response.Cookies.Add(cartinfo);

            var cart = this.GetCartItems();
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x =>
            {
                x.Quantity = cart.Where(c => c.ProductStockId == x.ProductStockId).ToList().Sum(v => v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });
          
            productsInCart = productsInCart.Where(x => x.ProductStockId != productStockId).ToList();
            return PartialView("~/Views/Shared/_MiniCart.cshtml", productsInCart);
        }

        [HttpGet]
        public void AddTocartFromDetail(OnlineShop.Models.MiniCartModel model)
        {
            int stockid = productProvider.GetProductStockbyColorSize(model.ProductId,model.Quantity);
            if (stockid > default(int))
            {
                int qty = model.Quantity;
                model.Quantity = 1;
                model.ProductStockId = stockid;
                for (int i = 0; i < qty; i++)
                {
                    this.UpdateCookie(model);
                }
            }
            var cart =  this.GetCartItems(); ;
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x =>
            {
                x.Quantity = cart.Where(c => c.ProductStockId == x.ProductStockId).ToList().Sum(v => v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });

            Cart();
            //return PartialView("~/Views/Shared/_MiniCart.cshtml", productsInCart);
        }

        [HttpGet]
        public ActionResult Updatecart(string cart)
        {
            string newValue = string.Empty;
            string[] data = cart.Split('#');
            data = data.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0; i < data.Count(); i++)
            {
                string[] stock_Qty = data[i].Split(',');
                int quantity = Convert.ToInt16(stock_Qty[1]);
                for (int j = 1; j <= quantity; j++)
                {
                    newValue = newValue + stock_Qty[0] + ",1#";
                }
            }

            HttpCookie cartinfo = new HttpCookie(CommonConstants.Constants.cartcookieName);
            cartinfo[CommonConstants.Constants.cartStocktId] = newValue;
            cartinfo.Expires.Add(new TimeSpan(12, 0, 0));
            Response.Cookies.Add(cartinfo);
            return RedirectToAction("cart");
        }

        [HttpPost]
        public PartialViewResult Addcart(OnlineShop.Models.MiniCartModel model)
        {
            var cart = this.UpdateCookie(model);
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x => 
            {
                x.Quantity = cart.Where(c=>c.ProductStockId==x.ProductStockId).ToList().Sum(v=>v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });

            return PartialView("~/Views/Shared/_MiniCart.cshtml", productsInCart);
        }

        public PartialViewResult GetCart()
        {
           var cart = this.GetCartItems();
            var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
            productsInCart.ForEach(x =>
            {
                x.Quantity = cart.Where(c => c.ProductStockId == x.ProductStockId).ToList().Sum(v => v.Quantity);
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;
            });
            return PartialView("~/Views/Shared/_MiniCart.cshtml", productsInCart);
        }

        #region Private
        private List<OnlineShop.Models.MiniCartModel> GetCartItems()
        {
            List<OnlineShop.Models.MiniCartModel> list = new List<Models.MiniCartModel>();
            HttpCookie reqCookies = Request.Cookies[CommonConstants.Constants.cartcookieName];
            if (reqCookies != null)
            {
                string stockIds = string.Empty;
                stockIds = reqCookies[CommonConstants.Constants.cartStocktId].ToString();
                if (stockIds != string.Empty)
                {
                    string[] stock = stockIds.Split('#');
                    stock = stock.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    for (int i = 0; i < stock.Count(); i++)
                    {
                        OnlineShop.Models.MiniCartModel cart = new Models.MiniCartModel();
                        var stockId_Qty = stock[i].Split(',');
                        if(stockId_Qty.Length>1)
                        { 
                        cart.ProductStockId = Convert.ToInt16(stockId_Qty[0]);
                        cart.Quantity = Convert.ToInt16(stockId_Qty[1]);

                        list.Add(cart);
                        }
                    }
                }
            }
            return list;
        }

        private List<OnlineShop.Models.MiniCartModel> UpdateCookie(OnlineShop.Models.MiniCartModel model)
        {
            List<OnlineShop.Models.MiniCartModel> list = new List<Models.MiniCartModel>();
            HttpCookie reqCookies = Request.Cookies[CommonConstants.Constants.cartcookieName];


            if (reqCookies == null)
            {
                    HttpCookie cartinfo = new HttpCookie(CommonConstants.Constants.cartcookieName);
                    cartinfo[CommonConstants.Constants.cartStocktId] = model.ProductStockId.ToString() + "," + model.Quantity.ToString();
                    cartinfo.Expires.Add(new TimeSpan(12, 0, 0));
                    Response.Cookies.Add(cartinfo);

                    list.Add(new Models.MiniCartModel
                    {
                        ProductStockId = model.ProductStockId,
                        Quantity = model.Quantity
                    });
            }
            else if (reqCookies != null )
            {
                string stockstring = string.Empty;
                stockstring = reqCookies[CommonConstants.Constants.cartStocktId].ToString();
                if (stockstring != string.Empty)
                {
                    string stockIds = string.Empty;
                    stockIds = reqCookies[CommonConstants.Constants.cartStocktId].ToString();

                    string newValues = stockIds + "#" + model.ProductStockId.ToString() + "," + model.Quantity.ToString();
                    string[] stock = newValues.Split('#');

                    for (int i = 0; i < stock.Count(); i++)
                    {
                        OnlineShop.Models.MiniCartModel cart = new Models.MiniCartModel();

                        var stockId_Qty = stock[i].Split(',');
                        stock = stock.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        if (stockId_Qty.Length == 1)
                        {
                            stockId_Qty = stock[i].Split(',');
                        }
                        cart.ProductStockId = Convert.ToInt16(stockId_Qty[0]);
                        cart.Quantity = Convert.ToInt16(stockId_Qty[1]);

                        list.Add(cart);
                    }
                    HttpCookie cartinfo = new HttpCookie(CommonConstants.Constants.cartcookieName);
                    cartinfo[CommonConstants.Constants.cartStocktId] = newValues;

                    cartinfo.Expires.Add(new TimeSpan(12, 0, 0));

                    Response.Cookies.Add(cartinfo);
                }
                else
                {
                    HttpCookie cartinfo = new HttpCookie(CommonConstants.Constants.cartcookieName);
                    cartinfo[CommonConstants.Constants.cartStocktId] = model.ProductStockId.ToString() + "," + model.Quantity.ToString();
                    cartinfo.Expires.Add(new TimeSpan(12, 0, 0));
                    Response.Cookies.Add(cartinfo);

                    list.Add(new Models.MiniCartModel
                    {
                        ProductStockId = model.ProductStockId,
                        Quantity = model.Quantity
                    });
                }
            }
            return list;
        }
        #endregion
    }
}