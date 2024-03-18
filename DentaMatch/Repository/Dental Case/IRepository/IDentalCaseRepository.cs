using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Repository;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalCaseRepository : IRepository<DentalCase>
    {
        IRepository<DentalDisease> DentalDiseases { get; }
        IRepository<CaseDentalDiseases> CaseDentalDiseases { get; }
        IRepository<CaseChronicDiseases> CaseChronicDiseases { get; }
        IRepository<ChronicDisease> ChronicDiseases { get; }
        IRepository<MouthImages> MouthImages { get; }
        IRepository<XrayIamges> XRayImages { get; }
        IRepository<PrescriptionImages> PrescriptionImages { get; }
        void UpdateDentalCaseProperties(DentalCase dentalCase, DentalCaseRequestVm model);
    }
}
