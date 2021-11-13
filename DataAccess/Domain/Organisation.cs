﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace DataAccess.Domain
{
    public class Organisation: BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; }

        public List<User> Users { get; set; }
        public List<Product> Products { get; set; }
        //public List<Order> Order { get; set; }
    }
}