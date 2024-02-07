//using System.ComponentModel.DataAnnotations;

//namespace DentaMatch.ViewModel.Authentication.Request
//{
//    public class SignInVM
//    {
//        [Required, StringLength(11, MinimumLength = 11)]
//        public string Phone { get; set; }
//        [Required]
//        public string Password { get; set; }
//    }
//    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//    {
//        bool emailProvided = !string.IsNullOrWhiteSpace(Email);
//        bool phoneProvided = !string.IsNullOrWhiteSpace(Phone) && Phone.Length == 11;

//        // Check if neither or both identifiers are provided
//        if (!(emailProvided ^ phoneProvided)) // XOR operation: true only if one is provided
//        {
//            yield return new ValidationResult("Either Email or Phone is required, but not both.", new[] { nameof(Email), nameof(Phone) });
//        }

//        // Additional validation for Phone can be added here, for example, to check if the format is correct
//        // if (phoneProvided && !Phone.StartsWith("01"))
//        // {
//        //     yield return new ValidationResult("The Phone number must start with '01'.", new[] { nameof(Phone) });
//        // }
//    }
//}
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class SignInVM : IValidatableObject
{
    [StringLength(11, MinimumLength = 11, ErrorMessage = "The Phone number must be 11 digits.")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool emailProvided = !string.IsNullOrWhiteSpace(Email);
        bool phoneProvided = !string.IsNullOrWhiteSpace(Phone) && Phone.Length == 11;

        // Check if neither or both identifiers are provided
        if (!(emailProvided ^ phoneProvided)) // XOR operation: true only if one is provided
        {
            yield return new ValidationResult("Either Email or Phone is required, but not both.", new[] { nameof(Email), nameof(Phone) });
        }
    }
}
