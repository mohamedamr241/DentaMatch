namespace DentaMatch.ViewModel
{
    public class AuthModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AuthModel<T> : AuthModel where T : class
    {
        public T Data { get; set; }
    }

}
