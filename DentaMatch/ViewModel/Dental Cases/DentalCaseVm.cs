using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseVm
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public List<IFormFile> MouthImages { get; set; }
        [Required]
        public List<IFormFile> XrayImages { get; set; }
        [Required]
        public List<IFormFile> PrescriptionImages { get; set; }
        [Required]
        public List<string> DentalDiseases { get; set; }
        [Required]
        public List<string> ChronicDiseases { get; set; }
    }
}
