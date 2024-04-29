using System.Linq.Expressions;

namespace DentaMatch.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, string? includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        void Update(T entity);
        int Count(Expression<Func<T, bool>> filter = null);
    }
}
