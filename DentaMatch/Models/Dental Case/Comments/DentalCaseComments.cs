using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DentaMatch.Models.Dental_Case.Comments
{
    public class DentalCaseComments
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("DentalCases"),Required, NotNull]
        public string CaseId { get; set; }
        public virtual DentalCase DentalCases { get; set; }

        [Required,NotNull]
        public string Comment {  get; set; }

        [ForeignKey("User"), Required, NotNull]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
