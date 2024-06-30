using DentaMatch.Services.FireBase.IServices;
using DentaMatch.ViewModel.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Controllers.Notifications
{
    [Authorize(Roles = "Patient, Doctor, AdminDoctor, Admin")]
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IFirebaseService _firebase;

        public NotificationController(IFirebaseService firebase)
        {
            _firebase = firebase;
        }

        [HttpPost]
        public IActionResult UserToken(FireBaseVM model)
        {
            try
            {
                var result = _firebase.StoreToken(model.userToken, model.userName);
                if (!result.Success)
                {
                    return BadRequest(new { Success = false, Message = $"failed to save token",Data = result });
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Success = false, Message = $"error in saving user token {ex}" });
            }
        }
    }
}
