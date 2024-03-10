namespace DentaMatch.ViewModel.Authentication.Response
{
    public class UserResponseVM
    {
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string? ProfileImage { get; set; }
        public string? ProfileImageLink { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string userName { get; set; }
    }
}
