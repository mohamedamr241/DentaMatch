using DentaMatch.Cache;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Comments;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.Comments.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.CodeAnalysis.Operations;
using System.CodeDom;

namespace DentaMatch.Services.Comments
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
        public async Task<AuthModel> createComment(string caseId, string comment, string commentorID, string Role)
        {
            try
            {
                var Case = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (Case == null)
                {
                    return new AuthModel { Success = false, Message = $"Dental case is not found!" };
                }
                var User = await _dentalunitOfWork.UserManager.FindByIdAsync(commentorID);
                if (User == null)
                {
                    return new AuthModel { Success = false, Message = $"User is not found!" };
                }
                var CaseComment = new DentalCaseComments()
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = caseId,
                    Comment = comment,
                    UserId = commentorID,
                    TimeStamp = DateTime.UtcNow
                };
                if (_cache.Retrieve(caseId) != null)
                {
                    List<DentalCaseCommentVM> cachedComments = (List<DentalCaseCommentVM>)_cache.Retrieve(caseId);

                    cachedComments.Add(new DentalCaseCommentVM()
                    {
                        id = CaseComment.Id,
                        Comment = comment,
                        fullName = User.FullName,
                        UserName = User.UserName,
                        Role = Role,
                        UserId = User.Id,
                        profileImageLink = User.ProfileImageLink,
                        TimeStamp = CaseComment.TimeStamp
                    });
                    _cache.Remove(caseId);
                    _cache.storeArrayInDays(caseId, cachedComments, 30);
                }
                else
                {
                    var result = _dentalunitOfWork.CaseCommentRepository.GetAll(u => u.CaseId == caseId, "User", orderBy: q => q.OrderBy(x => x.TimeStamp));
                    List<DentalCaseCommentVM> CaseComments = new List<DentalCaseCommentVM>();
                    if (result != null && result.Count() > 0)
                    {
                        CaseComments = await ConstructCommentsVM((List<DentalCaseComments>)result);

                    }
                    CaseComments.Add(new DentalCaseCommentVM()
                    {
                        id = CaseComment.Id,
                        Comment = comment,
                        fullName = User.FullName,
                        UserName = User.UserName,
                        Role = Role,
                        UserId= User.Id,
                        profileImageLink = User.ProfileImageLink,
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
        public AuthModel DeleteComment(string dentalCaseId, string commentId, string userId)
        {
            try
            {
                var comment = _dentalunitOfWork.CaseCommentRepository.Get(u => u.Id == commentId);
                if (comment == null)
                {
                    return new AuthModel { Success = false, Message = "comment not found!" };
                }
                if (comment.UserId != userId)
                {
                    return new AuthModel { Success = false, Message = "Your are not allowed to delete this comment" };
                }
                _dentalunitOfWork.CaseCommentRepository.Remove(comment);
                _dentalunitOfWork.Save();
                if (_cache.Retrieve(dentalCaseId) != null)
                {
                    List<DentalCaseCommentVM> cachedComments = (List<DentalCaseCommentVM>)_cache.Retrieve(dentalCaseId);

                    foreach( var comm in  cachedComments )
                    {
                        if( comm.id == commentId )
                        {
                            cachedComments.Remove( comm );
                            break;
                        }
                    }
                    _cache.Remove(dentalCaseId);
                    _cache.storeArrayInDays(dentalCaseId, cachedComments, 30);
                }
                return new AuthModel { Success = true, Message = "comment deleted successfully" };
            }
            catch (Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error deleting dental case comment: {ex.Message}" };
            }
        }
        public async Task<AuthModel<List<DentalCaseCommentVM>>> GetCaseComments(string caseId)
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
                    if (result != null && result.Count() > 0)
                    {
                        List<DentalCaseCommentVM> CaseComments = await ConstructCommentsVM((List<DentalCaseComments>)result);
                        _cache.storeArrayInDays(caseId, CaseComments, 30);
                        return new AuthModel<List<DentalCaseCommentVM>> { Success = true, Message = $"Getting comments Successfully", Data = CaseComments };
                    }
                }
                else
                {
                    List<DentalCaseCommentVM> cachedComments = (List<DentalCaseCommentVM>)_cache.Retrieve(caseId);
                    if (cachedComments != null && cachedComments.Count() > 0)
                    {
                        foreach (var item in cachedComments)
                        {
                            item.profileImageLink = (await _dentalunitOfWork.UserManager.FindByIdAsync(item.UserId)).ProfileImageLink;
                        }
                        return new AuthModel<List<DentalCaseCommentVM>> { Success = true, Message = $"Getting comments Successfully", Data = cachedComments };
                    }
                }
                return new AuthModel<List<DentalCaseCommentVM>> { Success = true, Message = $"No Comments" };
            }
            catch (Exception ex)
            {
                return new AuthModel<List<DentalCaseCommentVM>> { Success = false, Message = $"Error getting dentalCase Comments: {ex.Message}" };
            }
        }
        private async Task<List<DentalCaseCommentVM>> ConstructCommentsVM(List<DentalCaseComments> res)
        {
            List<DentalCaseCommentVM> CaseComments = new List<DentalCaseCommentVM>();
            foreach (var comment in res)
            {
                ApplicationUser userr = await _dentalunitOfWork.UserManager.FindByIdAsync(comment.UserId);
                string role = (await _dentalunitOfWork.UserManager.GetRolesAsync(userr))[0];
                var comm = new DentalCaseCommentVM()
                {
                    id = comment.Id,
                    fullName = comment.User.FullName,
                    UserId = comment.User.Id,
                    UserName = comment.User.UserName,
                    Comment = comment.Comment,
                    Role = role,
                    profileImageLink = userr.ProfileImageLink,
                    TimeStamp = comment.TimeStamp,
                };
                CaseComments.Add(comm);
            }
            return CaseComments;
        }
    }

}
