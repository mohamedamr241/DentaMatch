using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models
{
    public class DentalCase
    {
        [Key]
        public string Id { get; set; }
        [Required, MaxLength(350)]
        public string Description { get; set; }
        [Required]
        public bool IsKnown { get; set; } = false;
        [Required]
        public bool IsAssigned { get; set; } = false;

        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
