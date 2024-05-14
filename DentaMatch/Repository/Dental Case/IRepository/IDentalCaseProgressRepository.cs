using DentaMatch.Models.Dental_Case.CaseProgress;
using DentaMatch.Models;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalCaseProgressRepository : IRepository<DentalCaseProgress>
    {
        IRepository<DentalCase> DentalCases { get; }
        IEnumerable<DentalCaseProgress> GetAllProgress(string caseId);


    }
}
