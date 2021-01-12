using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ProductStock
    {
        public int ProductStockId { get; set; }
        public int CurrentPrice { get; set; }
        public decimal OldPrice { get; set; }
        public bool OnSale { get; set; }
        public bool IsFeatured { get; set; }
        public bool TopRated { get; set; }
        public int StockCount { get; set; }
        public string CurrencySymbol { get; set; }
        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public string ColourCode { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int ProductId { get; set; }
        public string ProductPrice { get; set; }
        public string OldProductPrice { get; set; }
        public int Discount { get; set; }
    }
}