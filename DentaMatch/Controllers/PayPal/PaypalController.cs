using Newtonsoft.Json;
using DentaMatch.Data;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DentaMatch.Services.Paypal;
using DentaMatch.Services.Paypal.IServices;

namespace DentaMatch.Controllers.PayPal
{
    [ApiController]
    [Route("[controller]")]
    public class PaypalController: ControllerBase
    {
        private readonly IPaypalServices _paypalService;
        private readonly IConfiguration _configuration;

        public PaypalController(IPaypalServices paypalService, IConfiguration configuration) 
        {
            _paypalService = paypalService;
            _configuration = configuration;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(double amount, string description)
        {
            try
            {
                var approvalLink = await _paypalService.CreatePayment(amount, description);
                return Ok(new { url = approvalLink });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("execute-payment")]
        public async Task<IActionResult> ExecutePayment(string paymentId, string token, string payerID)
        {
            try
            {
                var paymentState = await _paypalService.ExecutePayment(paymentId, token, payerID);
                if(paymentState == "approved")
                {
                    return Redirect($"{_configuration["AppUrl"]}/SuccessfulPayment.html");
                }
                else
                {
                    return Redirect($"{_configuration["AppUrl"]}/FailedPayment.html");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}