using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DentaMatch.Models
{
    public class ApplicationUser : IdentityUser
    {
        [ValidateNever]
        public string? ProfileImage { get; set; }

        [Required, MaxLength(80)]
        public string FullName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; } //0 -> male, 1 -> female
        [Required, MaxLength(100)]
        public string City { get; set; }
        [MaxLength(5)]
        public string VerificationCode { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime VerificationCodeTimeStamp { get; set; }

        public Boolean IsVerified { get; set; } = false;
    }
}
