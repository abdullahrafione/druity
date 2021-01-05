using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models
{
    public class OrderDetailModel
    {
        public int ProductStockId { get; set; }
        public int OrderStatusId { get; set; }
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }

    }
}