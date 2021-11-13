using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{

    public class OrderHistory
    {
        public OrderHistory()
        {
            EndDate = DateTime.Now;
            OrderStatuses = new List<OrderStatus>();
        }

        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreationDate { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public string CurrencySymbol { get; set; }
        public int ProductId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SearchText { get; set; }
        public string FilterValues { get; set; }
        public int UserId { get; set; }
        public bool IsFilter { get; set; }
        public List<OrderStatus> OrderStatuses { get; set; }

    }
}