using System.ComponentModel.DataAnnotations;

namespace DentaMatch.Models.Patient_Models.Dental_Case.Dental_Diseases
{
    public class DentalDisease
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string DiseaseName { get; set; }
    }
}
