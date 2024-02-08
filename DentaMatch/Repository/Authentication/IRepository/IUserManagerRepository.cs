using DentaMatch.Models;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IUserManagerRepository : IRepository<ApplicationUser>
    {
        void UpdateVerificationCode(ApplicationUser user, string verficationCode, bool isVerified = false);
        void UpdateProfilePicture(ApplicationUser user, string ImagePath);
    }
}
