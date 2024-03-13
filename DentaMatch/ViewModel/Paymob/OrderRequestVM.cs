using Newtonsoft.Json;

namespace DentaMatch.ViewModel.Paymob
{
    public class OrderRequestVM
    {
        [JsonProperty("auth_token")]
        public  string AuthToken { get; set; }

        [JsonProperty("delivery_needed")]
        public bool DeliveryNeeded { get; set; }

        [JsonProperty("amount_cents")]
        public  string AmountCents { get; set; }

        [JsonProperty("items")]
        public List<object> Items { get; set; } = new List<object>();

        [JsonProperty("merchant_order_id")]
        public  string MerchantOrderId { get; set; }

        public string TestAddingAttribute { get; set; }
    }
}
