using DentaMatch.Data;
using DentaMatch.IServices.Dental_Cases;
using DentaMatch.Models;
using DentaMatch.Models.Patient_Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Images;
using DentaMatch.Repository;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Services.Dental_Cases
{
    public class DentalCaseService : IDentalCaseService<DentalCaseResponseVM>
    {
        private readonly IDentalCaseUnitOfWork _unitOfWork;
        public DentalCaseService(IDentalCaseUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }
        public async Task<AuthModel<DentalCaseResponseVM>> CreateCaseAsync(string UserId, DentalCaseRequestVm model)
        {
            try
            {
                //var user = _db.Patients.Where(c => c.UserId == UserId).FirstOrDefault();
                var user = _unitOfWork.Patients.Get(c => c.UserId == UserId);
                if (user == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "User not found." };
                }

                if (!model.IsKnown && model.DentalDiseases.Count() > 0)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Your dental case must be known to enter your dental diseases" };
                }
                string patientId = user.Id;
                var dentalCase = new DentalCase
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = model.Description,
                    IsKnown = model.IsKnown,
                    PatientId = patientId.ToString()
                };

                _unitOfWork.DentalCases.Add(dentalCase);
                _unitOfWork.Save();
                //_db.DentalCases.Add(dentalCase);
                //_db.SaveChanges();

                var ChronicDiseasesIds = GetChronicDiseaseIds(model.ChronicDiseases);
                foreach (var ChronicDiseasesId in ChronicDiseasesIds)
                {
                    var chronicDisease = new CaseChronicDiseases
                    {
                        CaseId = dentalCase.Id,
                        DiseaseId = ChronicDiseasesId
                    };

                    //_db.CaseChronicDiseases.Add(chronicDisease);
                    _unitOfWork.CaseChronicDiseases.Add(chronicDisease);
                }

                var dentalDiseasesIds = GetDentalDiseaseIds(model.DentalDiseases);
                foreach (var dentalDiseasesId in dentalDiseasesIds)
                {
                    var dentalDisease = new CaseDentalDiseases
                    {
                        CaseId = dentalCase.Id,
                        DiseaseId = dentalDiseasesId
                    };

                    //_db.CaseDentalDiseases.Add(dentalDisease);
                    _unitOfWork.CaseDentalDiseases.Add(dentalDisease);
                }
                if (model.MouthImages.Count() < 4 || model.MouthImages.Count() > 6)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "The number of mouth images should be between 4 and 6, inclusive" };
                }
                if (model.XrayImages.Count() > 2)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Maximum Number of XRay Images is 2" };
                }
                if (model.PrescriptionImages.Count() > 2)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Maximum Number of Prescription Images is 2" };
                }

                List<string> MouthImagesPaths = new List<string>(); 
                List<string> XrayImagesPaths = new List<string>(); 
                List<string> PrescriptionImagesPaths = new List<string>(); 

                if (model.MouthImages != null)
                {
                  
                    foreach (var Mouthimage in model.MouthImages)
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
                            Image = @"\Images\MouthImages\" + fileName
                        };
                        //_db.MouthImages.Add(mouthImage);
                        _unitOfWork.MouthImages.Add(mouthImage);
                        MouthImagesPaths.Add(mouthImage.Image);
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
                    //_db.XrayIamges.Add(xrayImage);
                    _unitOfWork.XRayImages.Add(xrayImage);
                    XrayImagesPaths.Add(xrayImage.Image);

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
                    //_db.PrescriptionImages.Add(prescriptionImage);
                    _unitOfWork.PrescriptionImages.Add(prescriptionImage);
                    PrescriptionImagesPaths.Add(prescriptionImage.Image);

                }
                var dentalCaseData = new DentalCaseResponseVM
                {
                    Description = model.Description,
                    DentalDiseases = model.DentalDiseases.ToList(),
                    ChronicDiseases = model.ChronicDiseases.ToList(),
                    IsKnown = model.IsKnown,
                    MouthImages = MouthImagesPaths,
                    PrescriptionImages = PrescriptionImagesPaths,
                    XrayImages = XrayImagesPaths
                };
                _unitOfWork.Save();

                return new AuthModel<DentalCaseResponseVM>
                {
                    Success = true,
                    Message = "Dental Case Created successfully",
                    Data = dentalCaseData
                };

            }
            catch (Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        private List<string> GetChronicDiseaseIds(List<string> chronicDiseaseNames)
        {
            return _unitOfWork.ChronicDiseases
                .GetAll(cd => chronicDiseaseNames.Contains(cd.DiseaseName))
                .Select(cd => cd.Id)
                .ToList();
        }

        private List<string> GetDentalDiseaseIds(List<string> dentalDiseaseNames)
        {
            return _unitOfWork.DentalDiseases
                .GetAll(dd => dentalDiseaseNames.Contains(dd.DiseaseName))
                .Select(dd => dd.Id)
                .ToList();
        }
    }
}