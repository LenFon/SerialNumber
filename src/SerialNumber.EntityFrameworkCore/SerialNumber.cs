using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SerialNumber.EntityFrameworkCore
{
    [Table("SerialNumber")]
    public class SerialNumber
    {
        [Required]
        [StringLength(50)]
        [Key]
        public string Name { get; set; }

        public long Number { get; set; }
    }
}
