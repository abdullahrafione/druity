using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.Domain
{
    public class Order: BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        //public int OrganisationId { get; set; }
        //public Organisation Organisation { get; set; }

        public decimal TotalAmount { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Payment> Payment { get; set; }
        public string Guid { get; set; }
    }
}