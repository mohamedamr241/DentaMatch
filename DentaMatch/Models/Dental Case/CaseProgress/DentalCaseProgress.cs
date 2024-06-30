using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentaMatch.Models.Dental_Case.CaseProgress
{
    public class DentalCaseProgress
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("DentalCases"), Required]
        public string CaseId { get; set; }
        public virtual DentalCase DentalCases { get; set; }

        [Required]
        public string ProgressMessage { get; set; }

        public DateTime ProgressDate { get; set; } = DateTime.UtcNow;
    }
}