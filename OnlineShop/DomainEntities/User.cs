using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShop.DomainEntities
{
    public class User:BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(50)]
        public string Street { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(400)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        public string UniqueId { get; set; }

        public int OrganisationId { get; set; }
        public Organisation Organisation { get; set; }

        public List<Order> Order { get; set; }
    }
}