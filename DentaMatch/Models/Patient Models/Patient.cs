using DentaMatch.Models.Dental_Case.Reports;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models
{
    public class Patient
    {

        [Key, MaxLength(450)]
        public string Id { get; set; }
        [Required, MaxLength(150)]
        public string Address { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public virtual ICollection<DentalCase> PatientCases { get; set; }
        //public virtual ICollection<Report> PatientReports { get; set; }

    }
}
