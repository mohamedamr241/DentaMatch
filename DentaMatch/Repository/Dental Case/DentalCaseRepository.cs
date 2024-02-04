using DentaMatch.Data;
using DentaMatch.Models;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public class DentalCaseRepository<T> : Repository<T>, IDentalCaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        public DentalCaseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
