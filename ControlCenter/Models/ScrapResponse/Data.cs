using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models.ScrapResponse
{
    public class Data
    {
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public decimal price { get; set; }
        public string currency { get; set; }
        public bool isInStock { get; set; }
        public string EAN13 { get; set; }
        public object ASIN { get; set; }
        public object ISBN { get; set; }
        public string color { get; set; }
        public string brand { get; set; }
        public Category category { get; set; }
        public List<Category> categories { get; set; }
        public string siteURL { get; set; }
        public object siteHtml { get; set; }
        public object productHasVariations { get; set; }
        public object error { get; set; }
        public object statusCode { get; set; }
        public object isFinished { get; set; }
        public object isDead { get; set; }
        public int htmlLength { get; set; }
    }
}