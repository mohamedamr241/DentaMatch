namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IUserRepository<T> : IRepository<T> where T : class
    {
        void Update(T entity);
    }
}
