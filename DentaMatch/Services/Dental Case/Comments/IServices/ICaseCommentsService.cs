using DentaMatch.ViewModel.Dental_Cases;
using DentaMatch.ViewModel;

namespace DentaMatch.Services.Dental_Case.Comments.IServices
{
    public interface ICaseCommentsService
    {
        AuthModel createComment(string caseId, string comment, string commentorID);
        AuthModel<List<DentalCaseCommentVM>> GetCaseComments(string caseId);
    }
}
