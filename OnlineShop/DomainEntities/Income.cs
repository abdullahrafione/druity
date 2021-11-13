using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class Income:BaseEntity
    {
        public int AccountHeadId { get; set; }
        public AccountHead AccountHead { get; set; }
        public string Details { get; set; }
        public decimal Amount { get; set; }
    }
}