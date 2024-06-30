using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseResponseVM
    {
        public string CaseId { get; set; }
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public string DoctorName { get; set; }
        public string DoctorUniversity { get; set; }
        public int PatientAge { get; set; }
        public string PatientCity { get; set; }
        public string Description { get; set; }
        public bool IsKnown { get; set; }
        public bool IsAssigned { get; set; }
        public string PatientProfileImageLink { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string GoogleMapLink { get; set; }
        public List<string> ChronicDiseases { get; set; }
        public List<string> DentalDiseases { get; set; }
        public List<string> MouthImages { get; set; }
        public List<string> XrayImages { get; set; }
        public List<string> PrescriptionImages { get; set; }
    }
}
