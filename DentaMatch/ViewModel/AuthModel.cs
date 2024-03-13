
namespace DentaMatch.ViewModel
{
    public class AuthModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static implicit operator string(AuthModel v)
        {
            throw new NotImplementedException();
        }
    }

    public class AuthModel<T> : AuthModel 
    {
        public T Data { get; set; }
    }

}
