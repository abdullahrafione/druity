using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsSizeGuide { get; set; }
        public int DisplayOrder { get; set; }
    }
}