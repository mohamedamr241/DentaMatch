using DentaMatch.ViewModel.Paymob;
using DentaMatch.ViewModel;
using Newtonsoft.Json.Linq;

namespace DentaMatch.Services.Paymob.Iservice
{
    public interface IPaymobService
    {
        Task<AuthModel> GetPaymentToken(OrderPayMobVM order);
        Task<AuthModel> GetToken();
        Task<AuthModel> CreateOrder(string token, int TotalPrice);
        Task<AuthModel<object>> TransactionProcessedCallback(dynamic Payload, string hmac);
        bool VerifyHmac(Dictionary<string, string> data, string receivedHmac);
        Dictionary<string, string> ExtractHmacData(string payloadString);
        Dictionary<string, string> ExtractDataFromJson(JObject obj, List<string> keysToExtract);
    }
}
