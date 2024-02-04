using DentaMatch.Models;

namespace DentaMatch.Repository.IRepository
{
    public interface IUserManagerRepository : IRepository<ApplicationUser> 
    {
        void UpdateVerificationCode(ApplicationUser user, string verficationCode, bool isVerified = false);
    }
}
