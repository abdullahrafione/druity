using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ColorsSizes
    {
        public ColorsSizes()
        {
            Colours = new List<Colour>();
            Sizes = new List<Size>();
        }

        public int ProductId { get; set; }
        public List<Colour> Colours { get; set; }
        public List<Size> Sizes { get; set; }
    }
}