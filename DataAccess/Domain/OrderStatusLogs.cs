using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Domain
{
    public class OrderStatusLogs
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }

        public int OrderDetailId { get; set; }
        public OrderDetail OrderDetail { get; set; }
    }
}