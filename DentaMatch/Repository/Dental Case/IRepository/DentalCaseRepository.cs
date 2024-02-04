using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public class DentalCaseRepository : Repository<DentalCase>, IDentalCaseRepository
    {
        public DentalCaseRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
