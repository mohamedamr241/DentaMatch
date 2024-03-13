﻿using DentaMatch.Models;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        void UpdateVerificationCode(ApplicationUser user, string verficationCode, bool isVerified = false);
        void UpdateProfilePicture(ApplicationUser user, string? ImagePath = null, string? ImagePathLink = null);
        bool UpdateUserAccount(ApplicationUser user, UserUpdateRequestVM updatedUser);

    }
}
