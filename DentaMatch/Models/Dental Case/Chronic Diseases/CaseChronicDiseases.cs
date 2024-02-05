using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models.Dental_Case.Chronic_Diseases
{
    public class CaseChronicDiseases
    {
        [ForeignKey("DentalCases")]
        public string CaseId { get; set; }
        public virtual DentalCase DentalCases { get; set; }
        [ForeignKey("ChronicDiseases")]
        public string DiseaseId { get; set; }
        public virtual ChronicDisease ChronicDiseases { get; set; }

    }
}
