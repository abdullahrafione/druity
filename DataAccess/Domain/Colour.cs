﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Domain
{
    public class Colour
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<ProductStock> ProductStock { get; set; }
        public bool IsDeleted { get; set; }
    }
}