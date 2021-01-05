using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Domain
{
    public class Size
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductStock> ProductStock { get; set; }
        public bool IsDeleted { get; set; }
    }
}