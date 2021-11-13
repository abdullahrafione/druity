using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class GeneralImages : BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}