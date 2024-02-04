using DentaMatch.Models;

namespace DentaMatch.Repository.IRepository
{
    public interface IUserRepository<T> : IRepository<T> where T : class
    {
        void UpdateVerificationCode(T user, string verficationCode, bool isVerified = false);
    }
}
