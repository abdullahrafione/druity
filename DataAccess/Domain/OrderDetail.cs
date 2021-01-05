using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.Domain
{
    public class OrderDetail: BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductStockId { get; set; }
        public ProductStock ProductStock { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public int Quantity { get; set; }
        public decimal UnitSalePrice { get; set; }
        public List<OrderStatusLogs> OrderStatusLogs { get; set; }
    }
}