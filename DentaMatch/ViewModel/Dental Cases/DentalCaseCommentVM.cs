using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseCommentVM
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
    }
}
