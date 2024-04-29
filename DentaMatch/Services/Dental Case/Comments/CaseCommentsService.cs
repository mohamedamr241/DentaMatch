using DentaMatch.Cache;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Comments;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.Dental_Case.Comments.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using static DentaMatch.Services.Dental_Case.Comments.CaseCommentsService;

namespace DentaMatch.Services.Dental_Case.Comments
{
    public class CaseCommentsService : ICaseCommentsService
    {
        private readonly CacheItem _cache;
        private readonly IDentalUnitOfWork _dentalunitOfWork;
        public CaseCommentsService(IDentalUnitOfWork dentalunitOfWork, CacheItem cache)
        {
            _dentalunitOfWork = dentalunitOfWork;
            _cache = cache;
        }
        public AuthModel createComment(string caseId, string comment, string commentorID)
        {
            try
            {
                var Case =  _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (Case == null)
                {
                        return new AuthModel { Success = false, Message = $"Dental case is not found!" };
                }
                var User = _dentalunitOfWork.UserManager.FindByIdAsync(commentorID);
                if (User == null)
                {
                    return new AuthModel { Success = false, Message = $"Dental case is not found!" };
                }
                var CaseComment = new DentalCaseComments()
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = caseId,
                    Comment = comment,
                    UserId = commentorID,
                    TimeStamp = DateTime.UtcNow
                };
                if(_cache.Retrieve(caseId) != null)
                {
                    List<DentalCaseCommentVM> cachedComments = (List<DentalCaseCommentVM>)_cache.Retrieve(caseId);
                    
                    cachedComments.Add(new DentalCaseCommentVM()
                    {
                        id = CaseComment.Id,
                        Comment = comment,
                        fullName = User.Result.FullName,
                        TimeStamp = CaseComment.TimeStamp
                    });
                    _cache.Remove(caseId);
                    _cache.storeArrayInDays(caseId, cachedComments, 30);
                }
                else
                {
                    var result = _dentalunitOfWork.CaseCommentRepository.GetAll(u => u.CaseId == caseId, "User", orderBy: q => q.OrderBy(x => x.TimeStamp));
                    List<DentalCaseCommentVM> CaseComments = new List<DentalCaseCommentVM>();
                    if (result != null && result.Count()>0)
                    {
                         CaseComments = ConstructCommentsVM((List<DentalCaseComments>)result);

                    }
                    CaseComments.Add(new DentalCaseCommentVM()
                    {
                        id = CaseComment.Id,
                        Comment = comment,
                        fullName = User.Result.FullName,
                        TimeStamp = CaseComment.TimeStamp
                    });
                    _cache.storeArrayInDays(caseId, CaseComments, 30);
                }
                _dentalunitOfWork.CaseCommentRepository.Add(CaseComment);
                _dentalunitOfWork.Save();
                return new AuthModel { Success = true, Message = $"Comment is added Successfully" };
            }
            catch (Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error creating dental case comment: {ex.Message}" };
            }
        }
        public AuthModel<List<DentalCaseCommentVM>> GetCaseComments(string caseId)
        {
            try
            {
                var Case = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (Case == null)
                {
                    return new AuthModel<List<DentalCaseCommentVM>> { Success = false, Message = $"Dental case is not found!" };
                }
                if (_cache.Retrieve(caseId) == null)
                {
                    var result = _dentalunitOfWork.CaseCommentRepository.GetAll(u => u.CaseId == caseId, "User", orderBy: q => q.OrderBy(x => x.TimeStamp));
                    if(result!=null && result.Count()>0)
                    {
                        List<DentalCaseCommentVM> CaseComments = ConstructCommentsVM((List<DentalCaseComments>)result);
                        return new AuthModel<List<DentalCaseCommentVM>> { Success = true, Message = $"Getting comments Successfully", Data= CaseComments };
                    }
                }
                else
                {
                    List<DentalCaseCommentVM> cachedComments = (List<DentalCaseCommentVM>)_cache.Retrieve(caseId);
                    if (cachedComments!=null && cachedComments.Count()>0)
                    {
                        return new AuthModel<List<DentalCaseCommentVM>> { Success = true, Message = $"Getting comments Successfully", Data = cachedComments };
                    }
                }
                return new AuthModel<List<DentalCaseCommentVM>> { Success = true, Message = $"No Comments"};
            }
            catch(Exception ex)
            {
                return new AuthModel<List<DentalCaseCommentVM>> { Success = false, Message = $"Error getting dentalCase Comments: {ex.Message}" };
            }
        }
        private List<DentalCaseCommentVM> ConstructCommentsVM(List<DentalCaseComments> res)
        {
            List<DentalCaseCommentVM> CaseComments = new List<DentalCaseCommentVM>();
            foreach (var comment in res)
            {
                var comm = new DentalCaseCommentVM()
                {
                    id = comment.Id,
                    fullName = comment.User.FullName,
                    Comment = comment.Comment,
                    TimeStamp = comment.TimeStamp,
                };
                CaseComments.Add(comm);
            }
            return CaseComments;
        }
    }

}
