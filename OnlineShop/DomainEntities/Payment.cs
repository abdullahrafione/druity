using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class Payment:BaseEntity
    {
        public int PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public decimal Amount { get; set; }
        public string PaymentId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Rate { get; set; }
    }
}