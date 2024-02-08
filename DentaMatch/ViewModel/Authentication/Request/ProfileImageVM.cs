using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Request
{
    public class ProfileImageVM
    {
        [Required]
        public IFormFile? ProfileImage { get; set; }
    }
}
