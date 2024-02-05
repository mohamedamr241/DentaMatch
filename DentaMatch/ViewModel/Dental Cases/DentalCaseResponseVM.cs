using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseResponseVM
    {
        public string Description { get; set; }
        public bool IsKnown { get; set; }
        public bool IsAssigned { get; set; }
        public List<string> ChronicDiseases { get; set; }
        public List<string> DentalDiseases { get; set; }
        public List<string> MouthImages { get; set; }
        public List<string> XrayImages { get; set; }
        public List<string> PrescriptionImages { get; set; }

    }
}
