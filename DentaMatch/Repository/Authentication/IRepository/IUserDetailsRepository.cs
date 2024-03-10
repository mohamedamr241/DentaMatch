using DentaMatch.Models;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IUserDetailsRepository<T, K> where T : class where K : class
    {
        bool UpdateDetails(T userDetails, K user);
    }
}
