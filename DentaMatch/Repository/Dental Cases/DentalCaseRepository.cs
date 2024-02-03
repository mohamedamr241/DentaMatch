using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Patient_Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Images;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.CodeAnalysis.Operations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Repository.Dental_Cases
{
    public class DentalCaseRepository : IDentalCaseRepository
    {
        private readonly ApplicationDbContext _db;
        public DentalCaseRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<AuthModel<DentalCaseVm>> CreateCaseAsync(string UserId, DentalCaseVm model)
        {
            try
            {
                var user = _db.Patients.Where(c => c.UserId == UserId).FirstOrDefault();
                if(user == null)
                {
                    return new AuthModel<DentalCaseVm> { Success = false, Message = "User not found." };
                }
                string patientId = user?.Id;
                var dentalCase = new DentalCase
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = model.Description,
                    PatientId = patientId.ToString()
                };

                _db.DentalCases.Add(dentalCase);
                _db.SaveChanges();

                var ChronicDiseasesIds = GetChronicDiseaseIds(model.ChronicDiseases);
                foreach (var ChronicDiseasesId in ChronicDiseasesIds)
                {
                    var chronicDisease = new CaseChronicDiseases
                    {
                        CaseId = dentalCase.Id,
                        DiseaseId = ChronicDiseasesId
                    };

                    _db.CaseChronicDiseases.Add(chronicDisease);
                }

                var dentalDiseasesIds = GetDentalDiseaseIds(model.DentalDiseases);
                foreach (var dentalDiseasesId in dentalDiseasesIds)
                {
                    var dentalDisease = new CaseDentalDiseases
                    {
                        CaseId = dentalCase.Id,
                        DiseaseId = dentalDiseasesId
                    };

                    _db.CaseDentalDiseases.Add(dentalDisease);
                }
                if (model.MouthImages.Count() < 4)
                {
                    return new AuthModel<DentalCaseVm> { Success = false, Message = "Minimum Number of mouth Images is 4" };
                }
                if (model.MouthImages.Count() >6)
                {
                    return new AuthModel<DentalCaseVm> { Success = false, Message = "Maximum Number of mouth Images is 6" };
                }
                if (model.XrayImages.Count() > 2)
                {
                    return new AuthModel<DentalCaseVm> { Success = false, Message = "Maximum Number of XRay Images is 2" };
                }
                if (model.PrescriptionImages.Count() > 2)
                {
                    return new AuthModel<DentalCaseVm> { Success = false, Message = "Maximum Number of Prescription Images is 2" };
                }
                if (model.MouthImages != null )
                {
                    foreach( var Mouthimage in model.MouthImages)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Mouthimage.FileName);
                        string ImagePath = @"Images\MouthImages";

                        //if (!string.IsNullOrEmpty(productVm.product.ImageUrl))
                        //{
                        //    var imagePath = Path.Combine(wwwRootPath, productVm.product.ImageUrl.TrimStart('\\'));
                        //    if (System.IO.File.Exists(imagePath))
                        //    {
                        //        System.IO.File.Delete(imagePath);
                        //    }
                        //}
                        using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                        {
                            Mouthimage.CopyTo(fileStream);
                        }
                        var mouthImage = new MouthImages
                        {
                            Id = Guid.NewGuid().ToString(),
                            CaseId = dentalCase.Id,
                            Image= @"\Images\MouthImages\" + fileName
                        };
                        _db.MouthImages.Add(mouthImage);
                    }
                }
                foreach (var xrayimage in model.XrayImages)
                {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(xrayimage.FileName);
                        string ImagePath = @"Images\XRayImages";

                    //if (!string.IsNullOrEmpty(productVm.product.ImageUrl))
                    //{
                    //    var imagePath = Path.Combine(wwwRootPath, productVm.product.ImageUrl.TrimStart('\\'));
                    //    if (System.IO.File.Exists(imagePath))
                    //    {
                    //        System.IO.File.Delete(imagePath);
                    //    }
                    //}
                    using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                        {
                            xrayimage.CopyTo(fileStream);
                        }
                        var xrayImage = new XrayIamges
                        {
                            Id = Guid.NewGuid().ToString(),
                            CaseId = dentalCase.Id,
                            Image = @"\Images\XRayImages\" + fileName
                        };
                        _db.XrayIamges.Add(xrayImage);
                
                }
                foreach (var prescriptionimage in model.PrescriptionImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(prescriptionimage.FileName);
                    string ImagePath = @"Images\PrescriptionImages";

                    //if (!string.IsNullOrEmpty(productVm.product.ImageUrl))
                    //{
                    //    var imagePath = Path.Combine(wwwRootPath, productVm.product.ImageUrl.TrimStart('\\'));
                    //    if (System.IO.File.Exists(imagePath))
                    //    {
                    //        System.IO.File.Delete(imagePath);
                    //    }
                    //}
                    using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                    {
                        prescriptionimage.CopyTo(fileStream);
                    }
                    var prescriptionImage = new PrescriptionImages
                    {
                        Id = Guid.NewGuid().ToString(),
                        CaseId = dentalCase.Id,
                        Image = @"\Images\PrescriptionImages\" + fileName
                    };
                    _db.PrescriptionImages.Add(prescriptionImage);

                    
                }
                var dentalCaseData = new DentalCaseVm
                {
                    Description = model.Description,
                    DentalDiseases = model.DentalDiseases.ToList(),
                    ChronicDiseases = model.ChronicDiseases.ToList(),
                    MouthImages = model.MouthImages.ToList(),
                    PrescriptionImages = model.PrescriptionImages.ToList(),
                    XrayImages = model.XrayImages.ToList()
                };
                _db.SaveChanges();

                return new AuthModel<DentalCaseVm>
                {
                    Success = true,
                    Message = "Dental Case Created successfully",
                    Data  = dentalCaseData
                };

            }
            catch(Exception error)
            {
                return new AuthModel<DentalCaseVm> { Success = false, Message = $"{error.Message}" };
            }
        }

        private List<string> GetChronicDiseaseIds(List<string> chronicDiseaseNames)
        {
            return _db.ChronicDiseases
                .Where(cd => chronicDiseaseNames.Contains(cd.DiseaseName))
                .Select(cd => cd.Id)
                .ToList();
        }

        private List<string> GetDentalDiseaseIds(List<string> dentalDiseaseNames)
        {
            return _db.DentalDiseases
                .Where(dd => dentalDiseaseNames.Contains(dd.DiseaseName))
                .Select(dd => dd.Id)
                .ToList();
        }
    }
}