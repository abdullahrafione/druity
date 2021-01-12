using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models
{
    public class DashboardModel
    {
        public int ProductsCount { get; set; }
        public int UsersCount { get; set; }
        public int OrdersCount { get; set; }
        public int ActiveOrdersCount { get; set; }
        public int InProgressOrdersCount { get; set; }
        public int DeliveredOrdersCount { get; set; }
        public int DispatchedOrdersCount { get; set; }
        public int outOfStock { get; set; }
    }
}