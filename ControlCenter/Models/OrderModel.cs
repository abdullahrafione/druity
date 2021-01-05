using System;
using System.Collections.Generic;

namespace ControlCenter.Models
{
    public class OrderModel
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserEmail { get; set; }

    }
}