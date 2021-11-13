using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Domain
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreationTime = DateTime.Now;
            IsDeleted = false;
        }

        [Key]
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}