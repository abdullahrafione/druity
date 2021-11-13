using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class CategorySelecter
    {
        public int ParentSelectedCategoryId { get; set; }
        public bool IsMainSelected { get; set; }
        public int ChildSelectedCategory { get; set; }
        public bool IsChildSelected { get; set; }
    }
}