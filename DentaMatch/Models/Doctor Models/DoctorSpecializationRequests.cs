using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DentaMatch.Models.Doctor_Models
{
    public class DoctorSpecializationRequests
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("Doctor")]
        public string? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }
        [Required, NotNull]
        public string Specialization {  get; set; }
        [Required, NotNull]
        public bool IsVerified { get; set; } = false;
        [Required]
        public string Image { get; set; }
        [Required]
        public string ImageLink { get; set; }
    }
}
