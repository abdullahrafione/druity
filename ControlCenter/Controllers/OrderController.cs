using System.Web.Mvc;
using DataAccess.Providers;
using System.Linq;
using System.Collections.Generic;

namespace ControlCenter.Controllers
{
    public class OrderController : Controller
    {
        #region Constructors

        private readonly OrderProvider orderProvider;
        private readonly ProductProvider productProvider;

        public OrderController()
        {
            orderProvider = new OrderProvider();
            productProvider = new ProductProvider();
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

    }
}