using DentaMatch.Data;
using DentaMatch.Models.Doctor_Models;
using DentaMatch.Repository.Specialization.IRepository;

namespace DentaMatch.Repository.Specialization
{
    public class SpecializationRepository : Repository<DoctorSpecializationRequests>, ISpecializationRepository
    {
        private readonly ApplicationDbContext _db;
        public SpecializationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
