using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Repository.Dental_Case.IRepository;

namespace DentaMatch.Repository.Dental_Case
{
    public class DentalCaseUnitOfWork : IDentalCaseUnitOfWork
    {
        public DentalCaseRepository<DentalCase> DentalCases { get; set; }
        public DentalCaseRepository<Patient> Patients { get; set; }
        public DentalCaseRepository<Doctor> Doctors { get; set; }
        public DentalCaseRepository<CaseChronicDiseases> CaseChronicDiseases { get; set; }
        public DentalCaseRepository<CaseDentalDiseases> CaseDentalDiseases { get; set; }
        public DentalCaseRepository<MouthImages> MouthImages { get; set; }
        public DentalCaseRepository<XrayIamges> XRayImages { get; set; }
        public DentalCaseRepository<PrescriptionImages> PrescriptionImages { get; set; }
        public DentalCaseRepository<ChronicDisease> ChronicDiseases { get; set; }
        public DentalCaseRepository<DentalDisease> DentalDiseases { get; set; }

        private readonly ApplicationDbContext _db;
        public DentalCaseUnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            DentalCases = new DentalCaseRepository<DentalCase>(db);
            Patients = new DentalCaseRepository<Patient>(db);
            Doctors = new DentalCaseRepository<Doctor>(db);

            CaseChronicDiseases = new DentalCaseRepository<CaseChronicDiseases>(db);
            CaseDentalDiseases = new DentalCaseRepository<CaseDentalDiseases>(db);

            MouthImages = new DentalCaseRepository<MouthImages>(db);
            XRayImages = new DentalCaseRepository<XrayIamges>(db);
            PrescriptionImages = new DentalCaseRepository<PrescriptionImages>(db);

            ChronicDiseases = new DentalCaseRepository<ChronicDisease>(db);
            DentalDiseases = new DentalCaseRepository<DentalDisease>(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
