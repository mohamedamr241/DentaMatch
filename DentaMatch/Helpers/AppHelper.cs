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

            return fullPath;
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
    }
}
