namespace DentaMatch.ViewModel.Authentication.Request
{
    public class UserUpdateRequestVM
    {
        public IFormFile? ProfileImage { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string userName { get; set; }
    }
}
