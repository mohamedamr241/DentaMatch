using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models
{
    public class Doctor
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string University { get; set; }
        [Required]
        public string CardImage { get; set; }
        public string CardImageLink { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<DentalCase> DrAssignedCases { get; set; }
        public bool IsVerifiedDoctor { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
