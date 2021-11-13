using DataAccess.DbFactory;
using System.Linq;
using System.Collections.Generic;

namespace DataAccess.Providers
{
    public class OrderProvider
    {
        public List<Domain.Order> GetAllOrders()
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.Order.Include("User").OrderByDescending(x => x.CreationTime).ToList();
            }
        }

        public List<Domain.OrderDetail> GetOrderDetailByOrderId (int orderId)
        {
            using (DataDbContext context = new DataDbContext())
            {
                return context.OrderDetail.Include("OrderStatus").Include("ProductStock").Where(x => x.OrderId == orderId).ToList();
            }
        }

        public void UpdateOrderStatus (Domain.OrderDetail orderDetail)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var obj = context.OrderDetail.Where(x => x.Id == orderDetail.Id).FirstOrDefault();
                obj.OrderStatusId = orderDetail.OrderStatusId;
                context.SaveChanges();
            }
        }

        public int GenerateOrder (Domain.Order order)
        {
            using (DataDbContext context = new DataDbContext())
            {
                var orderAdded = context.Order.Add(order);
                context.SaveChanges();

                return orderAdded.Id;
            }
        }

        public void GenerateOrderDetail (Domain.OrderDetail order)
        {
            using (DataDbContext context = new DataDbContext())
            {
                context.OrderDetail.Add(order);
                context.SaveChanges();
            }
        }
    }
}