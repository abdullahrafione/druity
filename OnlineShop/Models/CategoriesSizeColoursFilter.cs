using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class CategoriesSizeColoursFilter
    {
        public CategoriesSizeColoursFilter()
        {
            Categories = new List<Category>();
            Sizes = new List<Size>();
            Colours = new List<Colour>();
            RetlatedCategories = new List<Category>();
        }
        public List<Category> Categories { get; set; }
        public List<Category> RetlatedCategories { get; set; }
        public List<Size> Sizes { get; set; }
        public List<Colour> Colours { get; set; }

        public int ParentSelectedCategoryId { get; set; }
        public bool IsMainSelected { get; set; }
        public int ChildSelectedCategory { get; set; }
        public bool IsChildSelected { get; set; }
    }
}