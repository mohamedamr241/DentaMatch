using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DentaMatch.Controllers.PayPal
{
    [Route("[controller]")]
    [ApiController]
    public class PaypalController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PaypalController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [HttpPost("create_order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            try
            {
                string accessToken = await GetAccessToken();
                if (accessToken == null)
                {
                    return StatusCode(500, "Failed to get access token");
                }

                var orderDataJson = new
                {
                    intent = orderRequest.Intent.ToUpper(),
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = "USD",
                                value = "100.00"
                            }
                        }
                    }
                };
                var data = JsonSerializer.Serialize(orderDataJson);
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var response = await client.PostAsync($"{_configuration["Url"]}/v2/checkout/orders", new StringContent(data, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("complete_order")]
        public async Task<IActionResult> CompleteOrder([FromBody] CompleteOrderRequest completeOrderRequest)
        {
            try
            {
                string accessToken = await GetAccessToken();
                if (accessToken == null)
                {
                    return StatusCode(500, "Failed to get access token");
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var response = await client.PostAsync($"{_configuration["Url"]}/v2/checkout/orders/{completeOrderRequest.OrderId}/{completeOrderRequest.Intent}", null);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<string> GetAccessToken()
        {
            var client = _httpClientFactory.CreateClient();
            var byteArray = Encoding.UTF8.GetBytes($"{_configuration["ClientId"]}:{_configuration["Secret"]}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var data = "grant_type=client_credentials";
            var response = await client.PostAsync($"{_configuration["Url"]}/v1/oauth2/token", new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded"));
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var accessToken = JsonSerializer.Deserialize<AccessTokenResponse>(responseContent)?.access_token;
            return accessToken;
        }
    }
    public class CompleteOrderRequest
    {
        public string OrderId { get; set; }
        public string Intent { get; set; }
    }

    public class AccessTokenResponse
    {
        public string access_token { get; set; }
    }

    public class OrderRequest
    {
        public string Intent { get; set; }
    }
}
