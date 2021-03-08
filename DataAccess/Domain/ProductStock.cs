using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.Domain
{
    public class ProductStock: BaseEntity
    {
        public decimal CurrentPrice { get; set; }
        public decimal OldPrice { get; set; }
        public decimal? Cost { get; set; }
        public bool OnSale { get; set; }
        public bool IsFeatured { get; set; }
        public bool TopRated { get; set; }
        public int StockCount { get; set; }
        public string CurrencySymbol { get; set; }

        public int ColourId { get; set; }
        public Colour Colour { get; set; }

        public int SizeID { get; set; }
        public Size Size { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}