using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Repository.Dental_Case
{
    public class DentalCaseRepository : Repository<DentalCase>, IDentalCaseRepository
    {

        public IRepository<DentalDisease> DentalDiseases { get; private set; }

        public IRepository<CaseDentalDiseases> CaseDentalDiseases { get; private set; }

        public IRepository<CaseChronicDiseases> CaseChronicDiseases { get; private set; }

        public IRepository<ChronicDisease> ChronicDiseases { get; private set; }

        public IRepository<MouthImages> MouthImages { get; private set; }

        public IRepository<XrayIamges> XRayImages { get; private set; }

        public IRepository<PrescriptionImages> PrescriptionImages { get; private set; }

        public DentalCaseRepository(ApplicationDbContext db) : base(db)
        {
            DentalDiseases = new Repository<DentalDisease>(db);
            CaseDentalDiseases = new Repository<CaseDentalDiseases>(db);
            CaseChronicDiseases = new Repository<CaseChronicDiseases>(db);
            ChronicDiseases = new Repository<ChronicDisease>(db);
            MouthImages = new Repository<MouthImages>(db);
            XRayImages = new Repository<XrayIamges>(db);
            PrescriptionImages = new Repository<PrescriptionImages>(db);
        }

        public void UpdateDentalCaseProperties(DentalCase dentalCase, DentalCaseRequestVm model)
        {
            if (dentalCase is not null)
            {
                dentalCase.Description = model.Description;
                dentalCase.IsKnown = model.IsKnown;
            }
        }
    }
}
