using DentaMatch.Data;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public class DentalCaseRepository<T> : Repository<T>, IDentalCaseRepository<T> where T : class
    {
        public DentalCaseRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
