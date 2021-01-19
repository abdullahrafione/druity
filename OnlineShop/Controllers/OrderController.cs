using Microsoft.Ajax.Utilities;
using OnlineShop.Auth;
using OnlineShop.DAL;
using OnlineShop.Models;
using OnlineShop.Paypal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    [Authorize]
    [AuthorizationFilter(CommonConstants.Constants.BuyerRole)]
    public class OrderController : Controller
    {
        private readonly ProductProvider productProvider;
        private readonly OrderProvider orderProvider;
        private readonly UserProvider userProvider;
        private readonly ImageProvider imageProvider;
        private readonly AccountController accountController;
        public OrderController()
        {
            orderProvider = new OrderProvider();
            productProvider = new ProductProvider();
            userProvider = new UserProvider();
            imageProvider = new ImageProvider();
            accountController = new AccountController();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            var user = (OnlineShop.Models.User)Session[CommonConstants.Constants.User];
            return View(user);
        }

        public ActionResult PlaceOrder()
        {
            try
            {
                var cart = this.GetCartItems();
                var productsInCart = productProvider.GetCart(cart.Select(x => x.ProductStockId).ToList());
                cart.ForEach(x =>
                {
                    x.Price = productsInCart.Where(c => c.ProductStockId == x.ProductStockId).FirstOrDefault().Price;
                    x.ProductName = productsInCart.Where(c => c.ProductStockId == x.ProductStockId).FirstOrDefault().ProductName;
                });

                var user = (OnlineShop.Models.User)Session[CommonConstants.Constants.User];
                var userId = userProvider.GetUserId(user.EmailAddress);
                var validatedCart = AvailablityCheck(productsInCart, cart);

                if (validatedCart != null)
                {
                    return RedirectToAction("cart", "shoppingcart");
                }
                else
                {
                    productProvider.ReduceProductStock(cart);
                    int orderid = orderProvider.GenerateOrder(cart, userId);
                    orderProvider.ActivateOrder(orderid);
                    orderProvider.PaymentStatusChanged(orderid, "Approved");
                    DeleteCart();
                    SendEmail(user.EmailAddress,"Order Placed", orderid);
                    return View("success");
                    //return RedirectToAction("PaymentWithPaypal", "order", new { orderid = orderid });
                }

            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [AllowAnonymous]
        public ActionResult PaymentWithPaypal(int? orderid = null, string Cancel = null)
        {
            var items = this.GetCartItems();
            var productsInCart = productProvider.GetCart(items.Select(x => x.ProductStockId).ToList());
            items.ForEach(x =>
            {
                x.Price = productsInCart.Where(c => c.ProductStockId == x.ProductStockId).FirstOrDefault().Price;
                x.ProductName = productsInCart.Where(c => c.ProductStockId == x.ProductStockId).FirstOrDefault().ProductName;
            });

            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {

                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/order/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    orderProvider.AddGuidinOrder(Convert.ToInt32(orderid), guid);

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, items, orderid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    int currentorder = orderProvider.GetOrderbyGuidOrder(guid);
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        productProvider.ReAddProductStock(items);
                        return View("FailureView");
                    }
                    orderProvider.ActivateOrder(currentorder);
                    orderProvider.PaymentStatusChanged(currentorder, "Approved");
                }
            }
            catch (Exception ex)
            {
                productProvider.ReAddProductStock(items);
                return View("FailureView");
            }

            DeleteCart();
            return View("success");
        }

        public ActionResult Purchasehistory()
        {
            return View();
        }

        public PartialViewResult GetOrderHistory()
        {
            var user = (OnlineShop.Models.User)Session[CommonConstants.Constants.User];
            int userId = userProvider.GetUserId(user.EmailAddress);
            var history = orderProvider.GetOrderHistory(userId);
            history.ForEach(x =>
            {
                x.ImageUrl = imageProvider.GetImages(x.ProductId).FirstOrDefault().Url;

            });
            return PartialView(history);
        }

        #region Private
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, List<MiniCartModel> items, int? orderid)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            decimal totalAmount = default(decimal);
            items.ForEach(x =>
            {
                totalAmount += x.Price * x.Quantity;
                itemList.items.Add(new Item()
                {
                    name = x.ProductName,
                    currency = "GBP",
                    price = x.Price.ToString(),
                    quantity = x.Quantity.ToString(),
                    sku = "sku"
                });
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = totalAmount.ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "GBP",
                total = (2 + totalAmount).ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Purchase from hussnain test store",
                invoice_number = orderid.ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
        private List<Models.MiniCartModel> AvailablityCheck(List<Models.MiniCartModel> availableProducts, List<Models.MiniCartModel> cart)
        {
            bool stopProcess = default(bool);

            cart.ForEach(x =>
            {
                var product = availableProducts.Where(c => c.ProductStockId == x.ProductStockId).FirstOrDefault();
                int qty = product.Quantity - x.Quantity;
                if (qty < 0)
                {
                    x.Message = "Available Quantity = " + product.Quantity;
                    stopProcess = true;
                }

            });
            if (stopProcess)
                return cart;
            return null;
        }
        private void GenerateOrderDetail(List<OnlineShop.Models.MiniCartModel> items, int orderId)
        {

        }
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
                        if (stockId_Qty.Length > 1)
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
        private void DeleteCart()
        {
            if (Request.Cookies[CommonConstants.Constants.cartcookieName] != null)
            {
                Response.Cookies[CommonConstants.Constants.cartcookieName].Expires = DateTime.Now.AddDays(-1);
            }
        }

        private void SendEmail(string receipent, string subject, int orderId)
        {
            string message = "Your Order has been placed successfully and your Order Id is: " + orderId;
            System.Net.Mail.MailMessage mail = new MailMessage();
            mail.To.Add(receipent);
            mail.From = new MailAddress("support@druity.com");
            mail.Subject = subject;
            mail.Body = message;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.sendgrid.net";
            smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
            smtp.Credentials = new System.Net.NetworkCredential("apikey", "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");
            smtp.EnableSsl = true;
            smtp.Send(mail);

        }
        #endregion
    }
}