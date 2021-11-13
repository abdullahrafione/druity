using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class PaymentStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Payment> Payment { get; set; }
    }
}