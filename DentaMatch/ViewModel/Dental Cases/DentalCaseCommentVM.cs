using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseCommentVM
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
    }
}
