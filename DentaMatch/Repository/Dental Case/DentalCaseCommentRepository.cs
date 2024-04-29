using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Comments;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Repository.Dental_Case.IRepository;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Dental_Case
{
    public class DentalCaseCommentRepository : Repository<DentalCaseComments>, IDentalCaseCommentRepository
    {
        public IRepository<DentalCase> DentalCases { get; private set; }

        public DentalCaseCommentRepository(ApplicationDbContext db) : base(db)
        {
            DentalCases = new Repository<DentalCase>(db);
        }

    }
}
