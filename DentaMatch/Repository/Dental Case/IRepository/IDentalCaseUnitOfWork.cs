using DentaMatch.Models.Patient_Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Images;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalCaseUnitOfWork
    {
        DentalCaseRepository<DentalCase> DentalCases { get; set; }
        DentalCaseRepository<Patient> Patients { get; set; }
        DentalCaseRepository<CaseChronicDiseases> CaseChronicDiseases { get; set; }
        DentalCaseRepository<CaseDentalDiseases> CaseDentalDiseases { get; set; }
        DentalCaseRepository<MouthImages> MouthImages { get; set; }
        DentalCaseRepository<XrayIamges> XRayImages { get; set; }
        DentalCaseRepository<PrescriptionImages> PrescriptionImages { get; set; }
        DentalCaseRepository<ChronicDisease> ChronicDiseases { get; set; }
        DentalCaseRepository<DentalDisease> DentalDiseases { get; set; }

        void Save();
    }
}
