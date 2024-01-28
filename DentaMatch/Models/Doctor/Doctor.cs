using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DentaMatch.Models.Doctor
{
    public class Doctor 
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
        [Required]
        public string University { get; set; }
        [Required, Display(Name = "Card Image")]
        public string CardImage { get; set; }
    }
}
