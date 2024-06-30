using DentaMatch.Models;
using DentaMatch.Models.Doctor_Models;

namespace DentaMatch.Repository.Specialization.IRepository
{
    public interface ISpecializationRepository : IRepository<DoctorSpecializationRequests>
    {
        void Save();
    }
}
