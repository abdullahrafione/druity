using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { get; set; }
        public DateTime CreationTime { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }

        //UserInfo
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }

        public List<Product> Products { get; set; }

    }
}