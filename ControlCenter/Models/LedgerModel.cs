using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models
{
    public class LedgerModel
    {
        public string Reference { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreationTime { get; set; }

    }
}