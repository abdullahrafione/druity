using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class Ledger: BaseEntity
    {
        public string Reference { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
    }
}