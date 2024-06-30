using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models.Dental_Case.Reports
{
    public class Report
    {
        [ForeignKey("DentalCase")]
        public string CaseId { get; set; }
        public virtual DentalCase DentalCase { get; set; }

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        public DateTime ReportTimestamp { get; set; }
    }
}
