using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class LandingPage
    {
        public List<Category> parentCategories { get; set; }
        public ProductReponse products { get; set; }
    }
}