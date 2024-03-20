using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseClassificationVM
    {
        [Required, NotNull]
        public string CaseId { get; set; }

        [Required, NotNull]
        public List<string> DentalDiseases { get; set; }
    }
}
