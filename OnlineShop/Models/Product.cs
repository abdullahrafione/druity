using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class Product
    {
        public Product()
        {
            ProductStock = new List<ProductStock>();
            Image = new List<Image>();
            ImageUrls = new List<string>();
        }
        public int OrganisationId { get; set; }
        public  string Name{ get; set; }
        public string Detail { get; set; }
        public bool isActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ShortDescription { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ProductStock> ProductStock { get; set; }
        public List<Image> Image { get; set; }
        public int ParentCategoryId { get; set; }
        public int GenderTagId { get; set; }
    }
}