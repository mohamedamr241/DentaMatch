using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseProgressVM
    {
        public string Id { get; set; }

        public string CaseId { get; set; }
        public string DoctorName { get; set; }

        public string ProgressMessage { get; set; }
        public DateTime Timestamp { get; set; }


    }
}