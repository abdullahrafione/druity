using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ProductReponse
    {
        public ProductReponse()
        {
            Products = new List<Product>();
        }
        public List<Product> Products { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }

        public int ParentSelectedCategoryId { get; set; }
        public bool IsMainSelected { get; set; }
        public int ChildSelectedCategory { get; set; }
        public bool IsChildSelected { get; set; }
    }
}