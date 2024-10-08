﻿using DentaMatch.ViewModel.Dental_Cases;
using DentaMatch.ViewModel;

namespace DentaMatch.Services.Comments.IServices
{
    public interface ICaseCommentsService
    {
        Task<AuthModel> createComment(string caseId, string comment, string commentorID, string Role);
        Task<AuthModel<List<DentalCaseCommentVM>>> GetCaseComments(string caseId);
        AuthModel DeleteComment(string dentalCaseId, string commentId, string userId);
    }
}
