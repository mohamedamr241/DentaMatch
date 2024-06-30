using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.CaseProgress;
using DentaMatch.Repository.Dental_Case.IRepository;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;


namespace DentaMatch.Repository.Dental_Case
{
    public class DentalCaseProgressRepository : Repository<DentalCaseProgress>, IDentalCaseProgressRepository
    {
        private readonly ApplicationDbContext _db;

        public IRepository<DentalCase> DentalCases { get; private set; }


        public DentalCaseProgressRepository(ApplicationDbContext db) : base(db)
        {
            DentalCases = new Repository<DentalCase>(db);
            _db = db;


        }
        public void UpdateCaseProgress(DentalCaseProgress model, string newProgress)
        {
            if(model != null && !string.IsNullOrEmpty(newProgress))
            {
                model.ProgressMessage = newProgress;

            }

        }

    }
}