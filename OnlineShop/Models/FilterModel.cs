using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class FilterModel
    {
        public string ParentCategories { get; set; }
        public string ChildCategories { get; set; }
        public string Sizes { get; set; }
        public string Colours { get; set; }
        public string SearchText { get; set; }
        public int CurrentPage { get; set; }
        public int GenderTagId { get; set; }
    }
}