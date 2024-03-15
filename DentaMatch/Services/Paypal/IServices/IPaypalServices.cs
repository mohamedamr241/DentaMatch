namespace DentaMatch.Services.Paypal.IServices
{
    public interface IPaypalServices
    {
        Task<string> CreatePayment(double amount, string description);
        Task<string> ExecutePayment(string paymentId, string token, string payerID);
    }
}
