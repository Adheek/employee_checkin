// LocationMaster.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("LocationMaster")]
    public class LocationMaster
    {
        [Key]
        public Guid LocationID { get; set; } = Guid.NewGuid();

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        public int LocationType { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; }
    }
}
