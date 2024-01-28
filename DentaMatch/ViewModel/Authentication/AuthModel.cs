namespace DentaMatch.ViewModel.Authentication
{
    public class AuthModel
    {
        public string Message { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string Token { get; set; }
        public bool IsAuth { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
