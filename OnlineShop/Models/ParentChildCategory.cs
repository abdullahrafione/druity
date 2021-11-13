using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ParentChildCategory
    {
        public ParentChildCategory()
        {
            ChildCategories = new List<Category>();
            ParentCategories = new List<Category>();
        }
        public List<Category> ParentCategories { get; set; }
        public int ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public List<Category> ChildCategories { get; set; }
    }
}