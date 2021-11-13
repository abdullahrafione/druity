using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public int DisplayOrder { get; set; }
        public int ParentCategoryId { get; set; }
        public List<Product> Products { get; set; }
    }
}