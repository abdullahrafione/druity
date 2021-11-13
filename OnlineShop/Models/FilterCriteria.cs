using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class FilterCriteria
    {
        public FilterCriteria()
        {
            ParentCategoryIds = new List<int>();
            ChildCategoryIds = new List<int>();
            SizeIds = new List<int>();
            ColourIds = new List<int>();
            GenderTagId = new List<int>();
        }
        public List<int> ParentCategoryIds { get; set; }
        public List<int> ChildCategoryIds { get; set; }
        public List<int> SizeIds { get; set; }
        public List<int> ColourIds { get; set; }
        public string SearchText { get; set; }
        public int CurrentPage { get; set; }

        public List<int> GenderTagId { get; set; }
    }
}