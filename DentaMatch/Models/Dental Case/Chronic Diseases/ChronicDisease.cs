using System.ComponentModel.DataAnnotations;

namespace DentaMatch.Models.Dental_Case.Chronic_Diseases
{
    public class ChronicDisease
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string DiseaseName { get; set; }
    }
}
