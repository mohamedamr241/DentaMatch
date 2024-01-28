using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DentaMatch.Models.Patient
{
    public class Patient
    {
        [Key]
        public int ID { get; set; }
        [ValidateNever]
        public string? ProfileImage { get; set; }

        [Required, MaxLength(50), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, MaxLength(50), Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; } //0 -> male, 1 -> female
        [Required, MaxLength(100)]
        public string Government { get; set; }


    }
}
