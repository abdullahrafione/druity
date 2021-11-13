﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
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