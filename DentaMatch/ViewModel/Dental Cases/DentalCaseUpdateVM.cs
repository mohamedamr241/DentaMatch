using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseUpdateVM
    {
        public string Description { get; set; }
        public bool IsKnown { get; set; }
        [ValidateNever]
        public List<IFormFile> MouthImages { get; set; }
        [ValidateNever]
        public List<IFormFile> XrayImages { get; set; }
        [ValidateNever]
        public List<IFormFile> PrescriptionImages { get; set; }
        [ValidateNever]
        public List<string> DentalDiseases { get; set; }
        [ValidateNever]
        public List<string> ChronicDiseases { get; set; }
    }
}
