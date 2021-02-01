
using System;

namespace ControlCenter.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Detail { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OldPrice { get; set; }
        public int StockCount { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public int productId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ImageUrl { get; set; }
    }
}