using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Comments;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalCaseCommentRepository : IRepository<DentalCaseComments>
    {
         IRepository<DentalCase> DentalCases { get;  }
    }
}
