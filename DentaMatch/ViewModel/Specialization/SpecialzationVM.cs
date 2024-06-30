using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Specialization
{
    public class SpecialzationVM
    {
        [Required]
        public string Specialization { get; set; }
        [Required]
        public IFormFile RequiredFile { get; set; }
    }
}
