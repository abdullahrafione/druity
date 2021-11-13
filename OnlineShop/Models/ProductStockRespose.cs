using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ProductStockRespose
    {
        public List<Category> Categories { get; set; }
        public List<Colour> Colours { get; set; }
        public List<Size> Sizes { get; set; }
        public Product Product { get; set; }
        public string ImageUrlPrimary { get; set; }
        public List<Image> Images { get; set; }
        public List<ProductStock> ProductStock { get; set; }
    }

}