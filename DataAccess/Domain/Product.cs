using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Domain
{
    public class Product: BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Detail { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int OrganisationId { get; set; }
        public Organisation Organisation { get; set; }

        public List<ProductStock> ProductStock { get; set; }
        public List<Image> Images { get; set; }
        public int GenderTagId { get; set; }
        public GenderTag GenderTag { get; set; }
    }
}