namespace DentaMatch.ViewModel.Authentication
{
   public class AuthModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}


