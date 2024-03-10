using DentaMatch.Models;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        void UpdateVerificationCode(ApplicationUser user, string verficationCode, bool isVerified = false);
        void UpdateProfilePicture(ApplicationUser user, string ImagePathLink, string ImagePath);
        bool UpdateUserAccount(ApplicationUser userDetails, ApplicationUser user);

    }
}
