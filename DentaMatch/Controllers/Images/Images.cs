using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DentaMatch.Controllers.Images
{

    [Route("[controller]")]
    [ApiController]
    public class Images : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public Images(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("GetImage")]
        public IActionResult GetImage([FromQuery] string imagePath)
        {
            try
            {

                string decodedPath = Uri.UnescapeDataString(imagePath);

                if (decodedPath.Contains(".."))
                {
                    return BadRequest("Invalid image path.");
                }


                string sanitizedPath = decodedPath.Replace('\\', '/');

                var fullPath = Path.Combine(_environment.WebRootPath, sanitizedPath);

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound(new { Success = false, Message = "Image not found." });
                }


                var contentType = "application/octet-stream"; 
                var extension = Path.GetExtension(fullPath).ToLowerInvariant();
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                        
                }


                return PhysicalFile(fullPath, contentType);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Dental Case Failed: {error.Message}" });
            }
        }
    }
}
