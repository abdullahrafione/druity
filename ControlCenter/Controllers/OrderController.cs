using System.Web.Mvc;
using DataAccess.Providers;
using System.Collections.Generic;
using System;

namespace ControlCenter.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        #region Constructors

        private readonly OrderProvider orderProvider;
        private readonly ProductProvider productProvider;
        private readonly UserProvider  userProvider;

        public OrderController()
        {
            orderProvider = new OrderProvider();
            productProvider = new ProductProvider();
            userProvider = new UserProvider();
        }

        #endregion

        public ActionResult Index()
        {
            var orders = orderProvider.GetAllOrders(); 

            return View(MaptoModel(orders));
        }

        public ActionResult UpdateOrderStatus(Models.OrderDetailModel model)
        {
            orderProvider.UpdateOrderStatus(MapforStatus(model));

            return RedirectToAction("Index");
        }

        public ActionResult ViewOrders(int orderId)
        {
            var orderDetail = orderProvider.GetOrderDetailByOrderId(orderId);
            TempData["OrderNumber"] = orderId;

            return View(ModelMapping(orderDetail));
        }

        public ActionResult ManualOrder()
        {
            var products = productProvider.GetProducts();

            return View(MapList(products));
        }

        [HttpPost]
        public ActionResult ManualOrder(Models.ManualOrderModel model)
        {
            var userId = userProvider.ManualUser(MapUser(model));
            var productStock = productProvider.GetStockByProductId(model.ProductId);
            var orderId = orderProvider.GenerateOrder(GeneratManualOrder(model, userId, productStock));
            orderProvider.GenerateOrderDetail(GenerateManualOrderDetails(orderId,productStock,model));

            return RedirectToAction("Index");
        }


        #region Private

        private List<Models.OrderModel> MaptoModel(List<DataAccess.Domain.Order> orders)
        {
            List<Models.OrderModel> mapped = new List<Models.OrderModel>();

            orders.ForEach(x =>
            {
                mapped.Add(new Models.OrderModel
                {
                    CreatedOn = x.CreationTime,
                    OrderId = x.Id,
                    TotalAmount = x.TotalAmount,
                    UserId = x.UserId,
                    UserEmail = x.User.EmailAddress
                });
            });

            return mapped;
        }

        private List<Models.OrderDetailModel> ModelMapping(List<DataAccess.Domain.OrderDetail> orders)
        {
            List<Models.OrderDetailModel> mapped = new List<Models.OrderDetailModel>();

            orders.ForEach(x =>
            {
                mapped.Add(new Models.OrderDetailModel
                {
                    Quantity = x.Quantity,
                    Status = x.OrderStatus.Name,
                    UnitPrice = x.UnitSalePrice,
                    ProductName = productProvider.GetProductByProductId(x.ProductStock.ProductId).Name,
                    ProductStockId = x.ProductStockId,
                    OrderDetailId = x.Id
                });
            });

            return mapped;
        }

        private DataAccess.Domain.OrderDetail MapforStatus (Models.OrderDetailModel model)
        {
            return new DataAccess.Domain.OrderDetail
            {
                Id = model.OrderDetailId,
                OrderStatusId = model.OrderStatusId
            };
        }

        #endregion

        #region ManualOrder

        private DataAccess.Domain.User MapUser (Models.ManualOrderModel model)
        {
            return new DataAccess.Domain.User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                EmailAddress = "ManualOrder@druity.com",
                Password = "MTEyMjMz",
                Phone = model.PhoneNo,
                Street = "N/A",
                PostalCode = "N/A",
                City = model.City,
                State = "N/A",
                Country = "Pakistan",
                OrganisationId = 2,
                CreationTime = DateTime.Now,
                IsActive = false,
                IsDeleted = false
            };
        }

        private Models.ManualOrderModel MapList(List<DataAccess.Domain.ProductStock> stock)
        {
            Models.ManualOrderModel mapped = new Models.ManualOrderModel();
            mapped.Products = new List<Models.Product>();
            stock.ForEach(x =>
            {
                mapped.Products.Add(new Models.Product
                {
                    Name = x.Product.Name,
                    CurrentPrice = x.CurrentPrice,
                    productId = x.Product.Id,
                    productStockId = x.Id
                });
            });

            return mapped;
        }

        private DataAccess.Domain.Order GeneratManualOrder(Models.ManualOrderModel model, int userId, DataAccess.Domain.ProductStock stock)
        {
            return new DataAccess.Domain.Order
            {
                CreationTime = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                UserId = userId,
                TotalAmount = model.Quantity * stock.CurrentPrice,
            };
        }

        private DataAccess.Domain.OrderDetail GenerateManualOrderDetails(int orderId, DataAccess.Domain.ProductStock stock, Models.ManualOrderModel model)
        {
            return new DataAccess.Domain.OrderDetail
            {
                OrderId = orderId,
                ProductStockId = stock.Id,
                OrderStatusId = 1,
                Quantity = model.Quantity,
                UnitSalePrice = stock.CurrentPrice,
                CreationTime = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };
        }

        #endregion

    }
}