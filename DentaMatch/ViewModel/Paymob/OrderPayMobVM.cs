using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Paymob
{
    public class OrderPayMobVM
    {
        [Required]
        public int TotalPrice { get; set; }
        [Required, MaxLength(80)]
        public string FirstName { get; set; }
        [Required, MaxLength(80)]
        public string LastName { get; set; }
        [Required, EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required, StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 digits")]
        public string phone_number { get; set; }
    }
}