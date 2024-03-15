using DentaMatch.Services.Paypal.IServices;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DentaMatch.Services.Paypal
{
    public class PaypalServices : IPaypalServices
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaypalServices(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> CreatePayment(double amount, string description)
        {
            var accessToken = await GetAccessToken();

            var createPaymentJson = new
            {
                intent = "sale",
                payer = new
                {
                    payment_method = "paypal"
                },
                redirect_urls = new
                {
                    return_url = $"{_configuration["AppUrl"]}Paypal/execute-payment",
                    cancel_url = $"{_configuration["AppUrl"]}/CancelPayment.html"
                },
                transactions = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency = "USD",
                            total = amount.ToString("0.00")
                        },
                        description = description
                    }
                }
            };
            _httpClient.DefaultRequestHeaders.Remove("Authorization");   // Clear any existing authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var createPaymentResponse = await SendPayPalRequest("/v1/payments/payment", createPaymentJson);
            return GetApprovalLink(createPaymentResponse.links);
        }

        public async Task<string> ExecutePayment(string paymentId, string token, string payerID)
        {
            var executePaymentJson = new
            {
                payer_id = payerID,
            };
            var executePaymentResponse = await SendPayPalRequest($"/v1/payments/payment/{paymentId}/execute?token={token}", executePaymentJson);
            return executePaymentResponse.state;
        }

        private async Task<string> GetAccessToken()
        {
            var clientId = _configuration["PaypalSettings:ClientId"];
            var clientSecret = _configuration["PaypalSettings:Secret"];

            var tokenRequest = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

            var response = await _httpClient.PostAsync($"{_configuration["PaypalSettings:Url"]}/v1/oauth2/token", tokenRequest);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return tokenData.access_token;
        }

        private async Task<dynamic> SendPayPalRequest(string endpoint, object requestData)
        {
            var requestJson = JsonConvert.SerializeObject(requestData);

            var fullUrl = new Uri(new Uri(_configuration["PaypalSettings:Url"]), endpoint);
            var accessToken = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Remove("Authorization");   // Clear any existing authorization header
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.PostAsync(fullUrl, new StringContent(requestJson, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(responseBody);
        }

        private string GetApprovalLink(dynamic links)
        {
            foreach (var link in links)
            {
                if (link.rel == "approval_url")
                {
                    return link.href;
                }
            }
            throw new Exception("Approval link not found in the PayPal API response.");
        }
    }
}
