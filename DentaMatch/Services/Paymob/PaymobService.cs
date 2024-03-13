using DentaMatch.Services.Paymob.Iservice;
using DentaMatch.ViewModel.Paymob;
using DentaMatch.ViewModel;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DentaMatch.Services.Paymob
{
    public class PaymobService : IPaymobService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PaymobService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<AuthModel> GetPaymentToken(OrderPayMobVM order)
        {
            var tokenResponse = await GetToken();
            if (!tokenResponse.Success)
            {
                return tokenResponse;
            }
            var orderResponse = await CreateOrder(tokenResponse.Message, order.TotalPrice);
            if (!orderResponse.Success)
            {
                return orderResponse;
            }
            var TotalPriceCents = order.TotalPrice * 100;
            var requestBody = new
            {
                auth_token = tokenResponse.Message,
                amount_cents = TotalPriceCents.ToString(),
                expiration = 3600,
                order_id = orderResponse.Message,
                billing_data = new BillingDataVM
                {
                    first_name = order.FirstName,
                    last_name = order.LastName,
                    email = order.Email,
                    phone_number = order.phone_number.ToString(),
                },
                currency = "EGP",
                integration_id = _configuration["PaymobSettings:PAYMOB_INTEGRATION_ID"],
                lock_order_when_paid = false
            };


            var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                string final_token = responseObject.token;
                return new AuthModel
                {
                    Success = true,
                    Message = final_token
                };
            }
            else
            {
                return new AuthModel
                {
                    Success = false,
                    Message = "Payment token error"
                };
            }
        }
         public async Task<AuthModel> GetToken()
         {
            var httpClient = _httpClientFactory.CreateClient();
            var apiKey = _configuration["PaymobSettings:PAYMOB_API_KEY"];

            var requestData = new Dictionary<string, string>
            {
                { "api_key", apiKey }
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseBody);
                string token = responseObject.token;
                return new AuthModel
                {
                    Success = true,
                    Message = token
                };
            }
            else
            {
                // Handle error
                return new AuthModel
                {
                    Success = false,
                    Message = "Authentication token error"
                };
            }
         }
        public async Task<AuthModel> CreateOrder(string token, int TotalPrice)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var items = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "name", "ASC1515" },
                    { "amount_cents", "500000" },
                    { "description", "Smart Watch" },
                    { "quantity", "1" }
                },
                new Dictionary<string, string>
                {
                    { "name", "ERT6565" },
                    { "amount_cents", "200000" },
                    { "description", "Power Bank" },
                    { "quantity", "1" }
                }
            };
            var orderPriceInCents = TotalPrice * 100;
            var requestData = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = orderPriceInCents.ToString(),
                currency = "EGP",
                items = items
            };

            var response = await httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", requestData);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseBody);
                string order_id = responseObject.id;
                return new AuthModel
                {
                    Success = true,
                    Message = order_id
                };
            }
            else
            {
                return new AuthModel
                {
                    Success = false,
                    Message = "Order creation error"
                };
            }
        }
        public async Task<AuthModel<object>> TransactionProcessedCallback(dynamic Payload, string hmac)
        {
            var payloadString = Payload.ToString();
            dynamic responseObject = JsonConvert.DeserializeObject(payloadString);
            TransactionDataVM transaction = JsonConvert.DeserializeObject<TransactionDataVM>(payloadString);

            Console.WriteLine(JsonConvert.SerializeObject(responseObject.obj, Formatting.Indented));

            var extractedData = ExtractHmacData(payloadString);

            var isValid = VerifyHmac(extractedData, hmac);

            if (!isValid)
            {
                return new AuthModel<object>
                {
                    Success = false,
                    Message = "Invalid HMAC"
                };
            }
            return new AuthModel<object>
            {
                Success = true,
                Data = responseObject
            };

        }
        public bool VerifyHmac(Dictionary<string, string> data, string receivedHmac)
        {
            StringBuilder concatenatedString = new StringBuilder();

            foreach (var pair in data)
            {
                concatenatedString.Append($"{pair.Value}");
            }

            string concatenatedData = concatenatedString.ToString();
            //var concatenatedData = string.Join("", data.OrderBy(kv => kv.Key).Select(kv => $"{kv.Value}"));
            var secretKey = _configuration["PaymobSettings:SecretKey"];

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedData));
                string calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return string.Equals(receivedHmac, calculatedHmac, StringComparison.OrdinalIgnoreCase);
            }
        }

        public Dictionary<string, string> ExtractHmacData(string payloadString)
        {
            JObject payloadObject = JObject.Parse(payloadString);

            // Extract the "obj" property
            JObject obj = payloadObject["obj"].ToObject<JObject>();

            // Deserialize the JSON into TransactionData object
            TransactionDataVM transactionData = obj.ToObject<TransactionDataVM>();
            // Create a dictionary to store the extracted data
            var extractedData = new Dictionary<string, string>
            {
                { "amount_cents", transactionData.amount_cents.ToString() },
                { "created_at", transactionData.created_at.ToString("yyyy-MM-ddTHH:mm:ss.ffffff") },
                { "currency", transactionData.currency },
                { "error_occured", transactionData.error_occured.ToString().ToLower() },
                { "has_parent_transaction", transactionData.has_parent_transaction.ToString().ToLower() },
                { "obj.id", transactionData.TransactionId.ToString() },
                { "integration_id", transactionData.integration_id.ToString() },
                { "is_3d_secure", transactionData.is_3d_secure.ToString().ToLower() },
                { "is_auth", transactionData.is_auth.ToString().ToLower() },
                { "is_capture", transactionData.is_capture.ToString().ToLower() },
                { "is_refunded", transactionData.is_refunded.ToString().ToLower() },
                { "is_standalone_payment", transactionData.is_standalone_payment.ToString().ToLower() },
                { "is_voided", transactionData.is_voided.ToString().ToLower() },
                { "order.id", transactionData.order.Id.ToString() },
                { "owner", transactionData.owner.ToString() },
                { "pending", transactionData.pending.ToString().ToLower() },
                { "source_data.pan", transactionData.source_data.pan },
                { "source_data.sub_type", transactionData.source_data.sub_type },
                { "source_data.type", transactionData.source_data.type },
                { "success", transactionData.success.ToString().ToLower() }
            };

            return extractedData;
        }

        public Dictionary<string, string> ExtractDataFromJson(JObject obj, List<string> keysToExtract)
        {
            var extractedData = new Dictionary<string, string>();

            foreach (var key in keysToExtract)
            {
                var property = obj.SelectToken(key.Replace(".", "['']").Replace("[", ".").Replace("]", ""));

                if (property != null)
                {
                    // Handle nested properties
                    if (key.StartsWith("source_data."))
                    {
                        var nestedObj = property as JObject;
                        if (nestedObj != null)
                        {
                            foreach (var nestedKey in nestedObj.Properties().Select(p => $"source_data.{p.Name}"))
                            {
                                extractedData[nestedKey] = nestedObj[nestedKey.Split('.').Last()].ToString();
                            }
                        }
                    }
                    else
                    {
                        extractedData[key] = property.ToString();
                    }
                }
            }

            return extractedData;
        }

        private Dictionary<string, string> ObjectToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                // Convert property value to string and add to dictionary
                dictionary.Add(prop.Name, prop.GetValue(obj)?.ToString());
            }

            return dictionary;
        }
    }
}
