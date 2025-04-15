// VendorMaster.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEET.Models
{
    [Table("VendorMaster")]
    public class VendorMaster
    {
        [Key]
        public Guid VendorID { get; set; } = Guid.NewGuid();

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(200)]
        public string VendorType { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; }
    }
}
