using System.Text;
using System.Text.RegularExpressions;

namespace DentaMatch.Helpers
{
    public class AppHelper
    {
        public string SaveImage(IFormFile image, string imagePath)
        {
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string fullPath = Path.Combine(imagePath, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return fileName;
        }

        public void DeleteImage(string imagePath)
        {
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), imagePath.TrimStart('\\'));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }

        public int GenerateThreeDigitsCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(100, 1000);
            return randomNumber;
        }

        public int GenerateCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 100000);
            return randomNumber;
        }
        public bool ContainsNonEnglishCharacters(string input)
        {
            // Regular expression to match non-English characters
            Regex regex = new Regex(@"[^\u0000-\u007F]+");
            return regex.IsMatch(input);
        }

        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
