using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class MiniCartModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductStockId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string CurrencySymbol { get; set; }
        public string ImageUrl { get; set; }
        public string SizeName { get; set; }
        public string ColorName { get; set; }

        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public string Message { get; set; }
    }
}