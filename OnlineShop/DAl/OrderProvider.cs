using OnlineShop.DbFactory;
using OnlineShop.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DAL
{
    public class OrderProvider
    {
        #region order
        public int GenerateOrder(List<Models.MiniCartModel> items, int userId)
        {
            decimal total = default(decimal);
            items.ForEach(x => { total = total + (x.Quantity * x.Price); });
            Order order = new Order
            {
                CreationTime = DateTime.UtcNow,
                IsActive = false,
                UserId = userId,
                TotalAmount = total
            };

            int orderId = this.AddOrder(order);

            List<OrderDetail> detailList = new List<OrderDetail>();

            int statusid = this.GetOrderStatus().Where(x => x.Name == "Open").FirstOrDefault().Id;

            items.ForEach(x =>
            {
                OrderDetail detail = new OrderDetail
                {
                    CreationTime = DateTime.UtcNow,
                    OrderId = orderId,
                    IsActive = true,
                    Quantity = x.Quantity,
                    ProductStockId = x.ProductStockId,
                    UnitSalePrice = x.Price,
                    OrderStatusId = statusid
                };
                detailList.Add(detail);
            });

            AddOrderDetail(detailList);

            this.AddPayment(orderId, total);

            return orderId;
        }

        public int AddOrder(Order order)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var addedOrder = context.Order.Add(order);
                context.SaveChanges();
                return addedOrder.Id;
            }
        }

        public void AddOrderDetail(List<OrderDetail> detail)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var addedOrder = context.OrderDetail.AddRange(detail);
                context.SaveChanges();
            }
        }

        public List<Models.OrderStatus> GetOrderStatus()
        {
            List<Models.OrderStatus> list = new List<Models.OrderStatus>();
            using (ShopDbContext context = new ShopDbContext())
            {
                var statuses = context.OrderStatus.ToList();
                statuses.ForEach(x =>
                {
                    list.Add(new Models.OrderStatus
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
                });
                return list;
            }
        }

        public void ChangeStatus(int statusid, int orderDetailId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var orderDetail = context.OrderDetail.Where(x => x.Id == orderDetailId).FirstOrDefault();
                orderDetail.OrderStatusId = statusid;
                context.SaveChanges();
            }
        }

        public void ActivateOrder(int orderId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var order = context.Order.Where(x => x.Id == orderId).FirstOrDefault();
                order.IsActive = true;
                context.SaveChanges();
            }
        }

        public void AddGuidinOrder(int orderid, string guid)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var order = context.Order.Where(x => x.Id == orderid).FirstOrDefault();

                order.Guid = guid;
                context.SaveChanges();
            }
        }

        public int GetOrderbyGuidOrder(string guid)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var order = context.Order.Where(x => x.Guid == guid).FirstOrDefault();
                return order.Id;
            }
        }

        public List<Models.OrderHistory> GetOrderHistory(int userId)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var query = (from ord in context.Order
                             join dtl in context.OrderDetail on ord.Id equals dtl.OrderId
                             join ps in context.ProductStock on dtl.ProductStockId equals ps.Id
                             join clr in context.Colour on ps.ColourId equals clr.Id
                             join size in context.Size on ps.SizeID equals size.Id
                             join prod in context.Product on ps.ProductId equals prod.Id
                             join pymt in context.Payment on ord.Id equals pymt.OrderId

                             where ord.IsActive == true
                             && ord.UserId == userId
                             && pymt.PaymentStatusId == 2

                             select new { ord, prod, ps, dtl });

                var result = new List<Models.OrderHistory>(query
                 .Select(prod => new Models.OrderHistory
                 {
                     OrderId = prod.ord.Id,
                     ProductName = prod.prod.Name,
                     ColorName = prod.ps.Colour.Name,
                     SizeName = prod.ps.Size.Name,
                     CreationDate = prod.ord.CreationTime,
                     Quantity = prod.dtl.Quantity,
                     UnitPrice = prod.dtl.UnitSalePrice,
                     Status = prod.dtl.OrderStatus.Name,
                     ProductId = prod.prod.Id,
                     CurrencySymbol = prod.ps.CurrencySymbol
                 }));
                var reverseorder = result.OrderByDescending(x => x.CreationDate).ToList();
                return reverseorder;
            }
        }

        public List<Models.OrderHistory> GetOrdersAdmin(Models.OrderHistory orderHistory)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                var query = (from ord in context.Order
                             join user in context.User on ord.UserId equals user.Id
                             join dtl in context.OrderDetail on ord.Id equals dtl.OrderId
                             join ps in context.ProductStock on dtl.ProductStockId equals ps.Id
                             join clr in context.Colour on ps.ColourId equals clr.Id
                             join size in context.Size on ps.SizeID equals size.Id
                             join prod in context.Product on ps.ProductId equals prod.Id
                             join pymt in context.Payment on ord.Id equals pymt.OrderId

                             where ord.IsActive == true
                             && pymt.PaymentStatusId == 2

                             select new { ord, prod, ps, dtl, user });

                var result = new List<Models.OrderHistory>(query
                 .Select(prod => new Models.OrderHistory
                 {
                     OrderId = prod.ord.Id,
                     OrderDetailId = prod.dtl.Id,
                     ProductName = prod.prod.Name,
                     ColorName = prod.ps.Colour.Name,
                     SizeName = prod.ps.Size.Name,
                     CreationDate = prod.ord.CreationTime,
                     Quantity = prod.dtl.Quantity,
                     UnitPrice = prod.dtl.UnitSalePrice,
                     Status = prod.dtl.OrderStatus.Name,
                     ProductId = prod.prod.Id,
                     CurrencySymbol = prod.ps.CurrencySymbol,
                     FirstName = prod.user.FirstName,
                     LastName = prod.user.LastName,
                     Email = prod.user.EmailAddress,
                     Address = prod.user.Address,
                     City = prod.user.City,
                     Country = prod.user.Country,
                     Phone = prod.user.Phone,
                     PostalCode = prod.user.PostalCode,
                     State = prod.user.State,
                     Street = prod.user.Street
                 }));
                var reverseorder = result.OrderByDescending(x => x.CreationDate).ToList();
                return ApplyFilter(orderHistory, reverseorder);
            }
        }

        #endregion

        #region payment
        public void AddPayment(int orderId, decimal amount)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                context.Payment.Add(new Payment
                {
                    Amount = amount,
                    CreationTime = DateTime.Now,
                    CurrencyCode = "GBP",
                    OrderId = orderId,
                    Rate = 1,
                    PaymentStatusId = context.PaymentStatus.Where(x => x.Name == "Pending").FirstOrDefault().Id
                });
                context.SaveChanges();
            }
        }

        public void PaymentStatusChanged(int orderId, string status)
        {
            using (ShopDbContext context = new ShopDbContext())
            {
                int statusId = context.PaymentStatus.Where(x => x.Name == status).FirstOrDefault().Id;

                var result = context.Payment.Where(x => x.OrderId == orderId).FirstOrDefault();
                result.PaymentStatusId = statusId;
                context.SaveChanges();
            }
        }

        #endregion

        #region Private

        private List<Models.OrderHistory> ApplyFilter(OnlineShop.Models.OrderHistory orderHistory, List<Models.OrderHistory> orders)
        {
            if (orderHistory.IsFilter && orders.Any())
            {
                List<Models.OrderHistory> filteredList = new List<Models.OrderHistory>();

                List<Models.OrderHistory> filteredByDate = new List<Models.OrderHistory>();

                if (orderHistory.StartDate != orderHistory.EndDate && orderHistory.StartDate < orderHistory.EndDate)
                {
                    filteredByDate = orders.Where(x => x.CreationDate >= orderHistory.StartDate && x.CreationDate <= orderHistory.EndDate).ToList();
                }

                List<Models.OrderHistory> filteredBySearchText = new List<Models.OrderHistory>();

                if (!string.IsNullOrEmpty(orderHistory.SearchText))
                {
                    filteredBySearchText = orders.Where(x => x.ProductName.Contains(orderHistory.SearchText)).ToList();
                }

                filteredList.AddRange(filteredByDate);
                filteredList.AddRange(filteredBySearchText);

                return filteredList;
            }

            return orders;
        }

        #endregion
    }
}