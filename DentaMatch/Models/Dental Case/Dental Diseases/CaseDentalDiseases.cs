using DentaMatch.Models.Dental_Case.Dental_Diseases;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models.Dental_Case.Dental_Diseases
{
    public class CaseDentalDiseases
    {
        [ForeignKey("DentalCases")]
        public string CaseId { get; set; }
        public virtual DentalCase DentalCases { get; set; }
        [ForeignKey("DentalDiseases")]
        public string DiseaseId { get; set; }
        public virtual DentalDisease DentalDiseases { get; set; }

    }
}
