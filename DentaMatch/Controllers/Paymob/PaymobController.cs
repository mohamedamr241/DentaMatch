using DentaMatch.Services.Paymob.Iservice;
using DentaMatch.ViewModel.Paymob;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Paymob
{
    [Route("[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly IPaymobService _paymob;
        private readonly IConfiguration _configuration;
        public PaymobController(IPaymobService paymob, IConfiguration configuration)
        {
            _paymob = paymob;
            _configuration = configuration;
        }

        [HttpPost("paymob")]
        public async Task<IActionResult> Credit(OrderPayMobVM order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var paymentTokenResponse = await _paymob.GetPaymentToken(order);
                if (!paymentTokenResponse.Success)
                {
                    return BadRequest(paymentTokenResponse);
                }
                return Ok(new { url = $"https://portal.weaccept.co/api/acceptance/iframes/{_configuration["PaymobSettings:FRAME-ID"]}?payment_token={paymentTokenResponse.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing payment: {ex.Message}");
            }

        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback(dynamic payload)
        {
            try
            {

                Dictionary<string, string> queryParams = new Dictionary<string, string>(Request.Query.Count);

                foreach (var query in Request.Query)
                {
                    queryParams.Add(query.Key, query.Value);
                }


                var response = await _paymob.TransactionProcessedCallback(payload, queryParams["hmac"]);
                if (!response.Success)
                {
                    return Redirect($"{_configuration["AppUrl2"]}/FailedPayment.html");
                }
                return Redirect($"{_configuration["AppUrl2"]}/SuccessfulPayment.html");
                //return Ok(response);
            }
            catch (Exception e)
            {

                return StatusCode(500, $"An error occurred while processing payment: {e.Message}");
            }
        }
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] Dictionary<string, string> queryParams)
        {
            try
            {
                if (queryParams["integration_id"] == _configuration["PaymobSettings:PAYMOB_INTEGRATION_ID"])
                {

                     return Redirect($"{_configuration["AppUrl2"]}/SuccessfulPayment.html");
                }
                else
                {
                    // Required parameters are missing
                    return Redirect($"{_configuration["AppUrl2"]}/FailedPayment.html");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing payment: {ex.Message}");
            }
        }
    }
}
