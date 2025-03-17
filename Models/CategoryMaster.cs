// CategoryMaster.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("CategoryMaster")]
    public class CategoryMaster
    {
        [Key]
        public Guid CategoryID { get; set; } = Guid.NewGuid();

        [StringLength(100)]
        public string CategoryName { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; }
    }
}
