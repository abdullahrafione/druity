using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCenter.Models
{
    public class Search
    {
        public int AccountHeadId { get; set; }
        public List<AccountHead> AccountHead { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string AccountHeadName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}