using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.Domain
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
    }
}