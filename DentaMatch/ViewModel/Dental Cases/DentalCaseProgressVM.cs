using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseProgressVM
    {
        [Required]
        public string DoctorId { get; set; }

        [Required]
        public string CaseId { get; set; }

        [Required]
        public string ProgressMessage { get; set; }
        public DateTime Timestamp { get; set; }


    }
}
