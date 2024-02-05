using DentaMatch.Repository.Dental_Case.IRepository;

namespace DentaMatch.Helpers
{
    public class DentalCaseHelper
    {
        private readonly IDentalCaseUnitOfWork _dentalCaseUnitOfWork;

        public DentalCaseHelper(IDentalCaseUnitOfWork dentalCaseUnitOfWork)
        {
            _dentalCaseUnitOfWork = dentalCaseUnitOfWork;
        }
        public List<string> GetChronicDiseaseIds(List<string> chronicDiseaseNames)
        {
            return _dentalCaseUnitOfWork.ChronicDiseases
                .GetAll(cd => chronicDiseaseNames.Contains(cd.DiseaseName))
                .Select(cd => cd.Id)
                .ToList();
        }

        public List<string> GetDentalDiseaseIds(List<string> dentalDiseaseNames)
        {
            return _dentalCaseUnitOfWork.DentalDiseases
                .GetAll(dd => dentalDiseaseNames.Contains(dd.DiseaseName))
                .Select(dd => dd.Id)
                .ToList();
        }

        public void DeleteImages(IEnumerable<string> imagePaths)
        {
            foreach (var imagePath in imagePaths)
            {
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), imagePath.TrimStart('\\'));

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
        }
    }
}
