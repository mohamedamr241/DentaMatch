using DentaMatch.Models;

namespace DentaMatch.Repository.Dental_Case
{
    public interface IDentalCaseRepository<T> : IRepository<T> where T : class
    {
    }
}
