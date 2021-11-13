using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models.ScrapResponse
{
    public class Root
    {
        public object error { get; set; }
        public Data data { get; set; }
    }
}