using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models
{
    public class Doctor
    {
        [Key, MaxLength(450)]
        public string Id { get; set; }

        [Required]
        public string University { get; set; }
        [Required, Display(Name = "Card Image")]
        public string CardImage { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<DentalCase> DrAssignedCases { get; set; }

    }
}
