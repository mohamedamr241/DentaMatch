using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Models.Dental_Case.Reports;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public IRepository<Report> Report { get; private set; }
        private readonly ApplicationDbContext _db;

        public DentalCaseRepository(ApplicationDbContext db) : base(db)
        {
            DentalDiseases = new Repository<DentalDisease>(db);
            CaseDentalDiseases = new Repository<CaseDentalDiseases>(db);
            CaseChronicDiseases = new Repository<CaseChronicDiseases>(db);
            ChronicDiseases = new Repository<ChronicDisease>(db);
            MouthImages = new Repository<MouthImages>(db);
            XRayImages = new Repository<XrayIamges>(db);
            PrescriptionImages = new Repository<PrescriptionImages>(db);
            Report = new Repository<Report>(db);
            _db = db;
        }
        public void updateDoctorRequestStatus(DentalCase request)
        {
            if (request == null)
            {
                request.DoctorId = null;
            }
        }


        public void UpdateDentalCaseProperties(DentalCase dentalCase, bool isKnown, string? description = null, DateTime? appointmentDateTime = null, string? googleMapLink = null)
        {
            if (dentalCase is not null)
            {
                if (description != null)
                    dentalCase.Description = description;

                dentalCase.IsKnown = isKnown;

                if (appointmentDateTime.HasValue)
                    dentalCase.AppointmentDateTime = appointmentDateTime.Value;

                dentalCase.GoogleMapLink = googleMapLink;
            }
        }

        public IEnumerable<DentalCase> FullTextSearch(Expression<Func<DentalCase, bool>> filter, string searchText, string? includeProperties = null)
        {
            DbSet<DentalCase> dbSet = _db.Set<DentalCase>();
            IQueryable<DentalCase> query = dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = query.Where(entity => EF.Functions.FreeText(entity.Description, searchText));

            return query.ToList();
        }
        public IEnumerable<DentalCase> GetFirstThreeCases(Expression<Func<DentalCase, bool>> filter = null, string? includeProperties = null)
        {
            IQueryable<DentalCase> query = _db.Set<DentalCase>();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Take(3).ToList();
        }
    }
}
