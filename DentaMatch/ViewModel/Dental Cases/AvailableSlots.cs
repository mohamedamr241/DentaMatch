using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class AvailableSlots
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public List<TimeOnly> Times { get; set; }
    }
}
