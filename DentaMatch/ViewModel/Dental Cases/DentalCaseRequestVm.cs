using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseRequestVm
    {
        public string Description { get; set; }
        public bool IsKnown { get; set; }
        [ValidateNever]
        public List<IFormFile>? MouthImages { get; set; }
        [ValidateNever]
        public List<IFormFile>? XrayImages { get; set; } 
        [ValidateNever]
        public List<IFormFile>? PrescriptionImages { get; set; }
        [ValidateNever]
        public List<string>? DentalDiseases { get; set; }
        [ValidateNever]
        public List<string>? ChronicDiseases { get; set; }
    }
}
