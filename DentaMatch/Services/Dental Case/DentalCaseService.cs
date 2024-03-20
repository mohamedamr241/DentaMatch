using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Services.Dental_Case
{
    public class DentalCaseService : IDentalCaseService
    {
        private readonly IDentalUnitOfWork _dentalunitOfWork;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IConfiguration _configuration;
        private readonly AppHelper _appHelper;
        public DentalCaseService(IDentalUnitOfWork dentalunitOfWork, IAuthUnitOfWork authUnitOfWork, AppHelper appHelper, IConfiguration configuration)
        {
            _dentalunitOfWork = dentalunitOfWork;
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
            _configuration = configuration;
        }
        public AuthModel<DentalCaseResponseVM> CreateCase(string UserId, DentalCaseRequestVm model)
        {
            try
            {
                var patient = _authUnitOfWork.PatientRepository.Get(u => u.UserId == UserId);

                if (patient == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "User not found" };
                }

                if (model.MouthImages is null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Mouth Images must be provided" };
                }

                var validationModelResult = ValidateCase(model);
                if (!validationModelResult.Success)
                {
                    return validationModelResult;
                }

                string patientId = patient.Id;
                var dentalCase = new DentalCase
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = model.Description,
                    IsKnown = model.IsKnown,
                    PatientId = patientId.ToString()
                };

                _dentalunitOfWork.DentalCaseRepository.Add(dentalCase);

                UpsertChronicDiseases(dentalCase, model.ChronicDiseases);
                UpsertDentalDiseases(dentalCase, model.DentalDiseases);
                UpsertMouthImages(dentalCase, model.MouthImages);
                UpsertXRayImages(dentalCase, model.XrayImages);
                UpsertPrescriptionImages(dentalCase, model.PrescriptionImages);

                _dentalunitOfWork.Save();

                var dentalCaseObj = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == dentalCase.Id, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
                var dentalCaseData = ConstructDentalCaseResponse(dentalCaseObj);

                return new AuthModel<DentalCaseResponseVM> { Success = true, Message = "Dental Case Created successfully", Data = dentalCaseData };

                }
            catch (Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error creating dental case: {error.Message}" };
            }
        }

        public AuthModel<DentalCaseResponseVM> UpdateCase(string caseId, DentalCaseRequestVm model)
        {
            try
            {
                var validationModelResult = ValidateCase(model);
                if (!validationModelResult.Success)
                {
                    return validationModelResult;
                }

                var dentalCase = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (dentalCase == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental Case Not Found" };
                }

                _dentalunitOfWork.DentalCaseRepository.UpdateDentalCaseProperties(dentalCase, model.IsKnown, model.Description);
                UpsertChronicDiseases(dentalCase, model.ChronicDiseases);
                UpsertDentalDiseases(dentalCase, model.DentalDiseases);
                UpsertMouthImages(dentalCase, model.MouthImages);
                UpsertXRayImages(dentalCase, model.XrayImages);
                UpsertPrescriptionImages(dentalCase, model.PrescriptionImages);

                _dentalunitOfWork.Save();

                var dentalCaseData = ConstructDentalCaseResponse(dentalCase);

                return new AuthModel<DentalCaseResponseVM> { Success = true, Message = "Dental Case updated successfully", Data = dentalCaseData };
            }
            catch (Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error updating dental case: {error.Message}" };
            }
        }

        public AuthModel DeleteCase(string caseId)
        {
            try
            {
                var dentalCase = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (dentalCase == null)
                {
                    return new AuthModel { Success = false, Message = "Dental Case Not Found" };
                }
                var mouthImagesPaths = _dentalunitOfWork.DentalCaseRepository.MouthImages.GetAll(u => u.CaseId == caseId).Select(u => u.Image);
                var XRayImagesPaths = _dentalunitOfWork.DentalCaseRepository.XRayImages.GetAll(u => u.CaseId == caseId).Select(u => u.Image);
                var PrescriptionImagesPaths = _dentalunitOfWork.DentalCaseRepository.PrescriptionImages.GetAll(u => u.CaseId == caseId).Select(u => u.Image);
                foreach (var imagePath in mouthImagesPaths.Concat(XRayImagesPaths).Concat(PrescriptionImagesPaths))
                {
                    _appHelper.DeleteImage(imagePath);
                }

                _dentalunitOfWork.DentalCaseRepository.Remove(dentalCase);
                _dentalunitOfWork.Save();
                return new AuthModel { Success = true, Message = "Dental Case deleted successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error Deleting dental case: {error.Message}" };
            }
        }
        public AuthModel<DentalCaseResponseVM> GetCase(string caseId)
        {
            try
            {
                var dentalCase = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (dentalCase == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental Case Not Found" };
                }
                var DentalCaseData = ConstructDentalCaseResponse(dentalCase);
                return new AuthModel<DentalCaseResponseVM>
                {
                    Success = true,
                    Message = "Dental Case retrieved successfully",
                    Data = DentalCaseData
                };
            }
            catch (Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error Retrieving dental case: {error.Message}" };
            }
        }

        public AuthModel<List<DentalCaseResponseVM>> GetPatientCases(string UserId)
        {
            try
            {
                var patient = _authUnitOfWork.PatientRepository.Get(c => c.UserId == UserId);
                if (patient == null)
                {
                    return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = "User not found." };
                }
                var DentalCases = _dentalunitOfWork.DentalCaseRepository.GetAll((u => u.PatientId == patient.Id), "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (DentalCases.Count() != 0)
                {
                    List<DentalCaseResponseVM> PatientDentalCases = new List<DentalCaseResponseVM>();
                    foreach (var DentalCase in DentalCases)
                    {
                        var PatientCase = ConstructDentalCaseResponse(DentalCase);
                        PatientDentalCases.Add(PatientCase);
                    }

                    return new AuthModel<List<DentalCaseResponseVM>>
                    {
                        Success = true,
                        Message = "Patient Dental Cases retrieved successfully",
                        Data = PatientDentalCases
                    };
                }
                return new AuthModel<List<DentalCaseResponseVM>>
                {
                    Success = true,
                    Message = "No Dental Cases Available",
                    Data = []
                };
            }
            catch (Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error Retrieving dental case: {error.Message}" };
            }
        }

        public AuthModel<List<DentalCaseResponseVM>> GetAssignedCases(string UserId)
        {
            try
            {
                var doctor = _authUnitOfWork.DoctorRepository.Get(c => c.UserId == UserId);
                if (doctor == null)
                {
                    return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = "User not found" };
                }
                var DentalCases = _dentalunitOfWork.DentalCaseRepository.GetAll((u => u.DoctorId == doctor.Id),
                    "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (DentalCases.Count() != 0)
                {
                    List<DentalCaseResponseVM> AssignedDentalCases = new List<DentalCaseResponseVM>();
                    foreach (var DentalCase in DentalCases)
                    {
                        var AssginedCase = ConstructDentalCaseResponse(DentalCase);
                        AssignedDentalCases.Add(AssginedCase);
                    }
                    return new AuthModel<List<DentalCaseResponseVM>>
                    {
                        Success = true,
                        Message = "Doctor Assigned Dental Cases retrieved successfully",
                        Data = AssignedDentalCases
                    };
                }
                return new AuthModel<List<DentalCaseResponseVM>>
                {
                    Success = true,
                    Message = "No Assigned Dental Cases Available",
                    Data = []
                };
            }
            catch (Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error Retrieving assigned doctor dental cases: {error.Message}" };
            }
        }

        public AuthModel<List<DentalCaseResponseVM>> GetUnAssignedCases()
        {
            try
            {
                var DentalCases = _dentalunitOfWork.DentalCaseRepository.GetAll((u => !u.IsAssigned),
                    "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (DentalCases!=null && DentalCases.Count() != 0)
                {
                    List<DentalCaseResponseVM> UnAssignedDentalCases = new List<DentalCaseResponseVM>();
                    foreach (var DentalCase in DentalCases)
                    {
                        var UnAssginedCase = ConstructDentalCaseResponse(DentalCase);
                        UnAssignedDentalCases.Add(UnAssginedCase);
                    }
                    return new AuthModel<List<DentalCaseResponseVM>>
                    {
                        Success = true,
                        Message = "UnAssigned Dental Cases retrieved successfully",
                        Data = UnAssignedDentalCases
                    };
                }
                return new AuthModel<List<DentalCaseResponseVM>>
                {
                    Success = true,
                    Message = "No Dental Cases Available",
                    Data = []
                };
            }
            catch (Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error Retrieving dental cases: {error.Message}" };
            }
        }
        public AuthModel<List<DentalCaseResponseVM>> GetUnkownCases()
        {
            try
            {
                var DentalCases = _dentalunitOfWork.DentalCaseRepository.GetAll((u => !u.IsKnown),
                    "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (DentalCases!= null && DentalCases.Count() != 0)
                {
                    List<DentalCaseResponseVM> UnAssignedDentalCases = new List<DentalCaseResponseVM>();
                    foreach (var DentalCase in DentalCases)
                    {
                        var UnAssginedCase = ConstructDentalCaseResponse(DentalCase);
                        UnAssignedDentalCases.Add(UnAssginedCase);
                    }
                    return new AuthModel<List<DentalCaseResponseVM>>
                    {
                        Success = true,
                        Message = "Unknown Dental Cases retrieved successfully",
                        Data = UnAssignedDentalCases
                    };
                }
                return new AuthModel<List<DentalCaseResponseVM>>
                {
                    Success = true,
                    Message = "No Dental Cases Available",
                    Data = []
                };
            }
            catch (Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error Retrieving Unkown dental cases: {error.Message}" };
            }
        }

        public AuthModel<List<DentalCaseResponseVM>> SearchByDentalDisease(string diseaseName)
        {
            try
            {
                var cases = _dentalunitOfWork.DentalCaseRepository.GetAll(
                    filter: c => c.CaseDentalDiseases.Any(cd => cd.DentalDiseases.DiseaseName == diseaseName),
                    includeProperties: "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User"
                );

                if (cases != null && cases.Count() != 0)
                {
                    List<DentalCaseResponseVM> UnAssignedDentalCases = new List<DentalCaseResponseVM>();
                    foreach (var DentalCase in cases)
                    {
                        var UnAssginedCase = ConstructDentalCaseResponse(DentalCase);
                        UnAssignedDentalCases.Add(UnAssginedCase);
                    }
                    return new AuthModel<List<DentalCaseResponseVM>>
                    {
                        Success = true,
                        Message = "dental cases are retrieved successfully",
                        Data = UnAssignedDentalCases
                    };
                }
                return new AuthModel<List<DentalCaseResponseVM>>
                {
                    Success = true,
                    Message = "No Dental Cases Available",
                    Data = []
                };

            }
            catch (Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error searching for dental cases: {error.Message}" };
            }
        }
        public AuthModel<List<DentalCaseResponseVM>> SearchByDescription(string query)
        {
            try
            {
                var cases = _dentalunitOfWork.DentalCaseRepository.FullTextSearch(
                    null,
                    searchText: query,
                    includeProperties: "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User"
                );
                if (cases != null && cases.Count() != 0)
                {
                    List<DentalCaseResponseVM> UnAssignedDentalCases = new List<DentalCaseResponseVM>();
                    foreach (var DentalCase in cases)
                    {
                        var UnAssginedCase = ConstructDentalCaseResponse(DentalCase);
                        UnAssignedDentalCases.Add(UnAssginedCase);
                    }
                    return new AuthModel<List<DentalCaseResponseVM>>
                    {
                        Success = true,
                        Message = "dental cases are retrieved successfully",
                        Data = UnAssignedDentalCases
                    };
                }
                return new AuthModel<List<DentalCaseResponseVM>>
                {
                    Success = true,
                    Message = "No Dental Cases Available",
                    Data = []
                };
            }
            catch(Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error searching for dental cases: {error.Message}" };

            }
        }
        public AuthModel ClassifyCase(DentalCaseClassificationVM model)
        {
            try
            {
                var PatientCase = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == model.CaseId);
                if(PatientCase == null)
                {
                    return new AuthModel { Success = false, Message = "Dental case is not found!" };
                }
                UpsertDentalDiseases(PatientCase, model.DentalDiseases);
                _dentalunitOfWork.DentalCaseRepository.UpdateDentalCaseProperties(PatientCase, true);
                _dentalunitOfWork.Save();
                return new AuthModel { Success = true, Message = "Dental Case is Classified Successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"Error classifying dental case: {error.Message}" };
            }
        }
        private void UpsertMouthImages(DentalCase dentalCase, List<IFormFile> mouthImages)
        {
            if (dentalCase.MouthImages != null)
            {
                foreach (var existingImage in dentalCase.MouthImages.ToList())
                {
                    _appHelper.DeleteImage(existingImage.Image);
                    _dentalunitOfWork.DentalCaseRepository.MouthImages.Remove(existingImage);
                }
            }

            if (mouthImages is not null)
            {
                string MouthImagesPath = Path.Combine("wwwroot", "Images", "DentalCase", "MouthImages");
                foreach (var mouthImage in mouthImages)
                {
                    string ImageName = _appHelper.SaveImage(mouthImage, MouthImagesPath);

                    var newMouthImage = new MouthImages
                    {
                        Id = Guid.NewGuid().ToString(),
                        CaseId = dentalCase.Id,
                        Image = Path.Combine(MouthImagesPath, ImageName),
                        ImageLink = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "DentalCase", "MouthImages", ImageName)
                    };

                    _dentalunitOfWork.DentalCaseRepository.MouthImages.Add(newMouthImage);
                }
            }
        }

        private void UpsertXRayImages(DentalCase dentalCase, List<IFormFile> xrayImages)
        {
            if (dentalCase.XrayImages is not null)
            {
                foreach (var existingImage in dentalCase.XrayImages.ToList())
                {
                    _appHelper.DeleteImage(existingImage.Image);
                    _dentalunitOfWork.DentalCaseRepository.XRayImages.Remove(existingImage);
                }
            }

            if (xrayImages is not null)
            {
                string XrayImagesPath = Path.Combine("wwwroot", "Images", "DentalCase", "XRayImages");
                foreach (var xrayImage in xrayImages)
                {
                    string ImageName = _appHelper.SaveImage(xrayImage, XrayImagesPath);

                    var newXrayImage = new XrayIamges
                    {
                        Id = Guid.NewGuid().ToString(),
                        CaseId = dentalCase.Id,
                        Image = Path.Combine(XrayImagesPath, ImageName),
                        ImageLink = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "DentalCase", "XRayImages", ImageName)
                    };

                    _dentalunitOfWork.DentalCaseRepository.XRayImages.Add(newXrayImage);
                }
            }
        }

        private void UpsertPrescriptionImages(DentalCase dentalCase, List<IFormFile> prescriptionImages)
        {
            if (dentalCase.PrescriptionImages != null)
            {
                foreach (var existingImage in dentalCase.PrescriptionImages.ToList())
                {
                    _appHelper.DeleteImage(existingImage.Image);
                    _dentalunitOfWork.DentalCaseRepository.PrescriptionImages.Remove(existingImage);
                }
            }

            if (prescriptionImages is not null)
            {
                string PrescriptionImagesPath = Path.Combine("wwwroot", "Images", "DentalCase", "PrescriptionImages");
                foreach (var prescriptionImage in prescriptionImages)
                {
                    string ImageName = _appHelper.SaveImage(prescriptionImage, PrescriptionImagesPath);

                    var newPrescriptionImage = new PrescriptionImages
                    {
                        Id = Guid.NewGuid().ToString(),
                        CaseId = dentalCase.Id,
                        Image = Path.Combine(PrescriptionImagesPath, ImageName),
                        ImageLink = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "DentalCase", "PrescriptionImages", ImageName)
                    };

                    _dentalunitOfWork.DentalCaseRepository.PrescriptionImages.Add(newPrescriptionImage);
                }
            }
        }

        private void UpsertChronicDiseases(DentalCase dentalCase, List<string> chronicDiseases)
        {
            if (dentalCase.CaseChronicDiseases is not null)
            {
                var existingChronicDiseases = _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases
                    .GetAll(u => u.CaseId == dentalCase.Id, "ChronicDiseases").ToList();

                _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.RemoveRange(existingChronicDiseases);
            }

            if (chronicDiseases is not null)
            {
                foreach (var chronicDiseaseName in chronicDiseases)
                {
                    var chronicDisease = _dentalunitOfWork.DentalCaseRepository.ChronicDiseases.Get(u => u.DiseaseName == chronicDiseaseName);

                    if (chronicDisease is not null)
                    {
                        CaseChronicDiseases caseChronicDisease = new CaseChronicDiseases
                        {
                            CaseId = dentalCase.Id,
                            DiseaseId = chronicDisease.Id
                        };

                        _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.Add(caseChronicDisease);
                    }
                }
            }
        }

        private void UpsertDentalDiseases(DentalCase dentalCase, List<string> dentalDiseases)
        {
            if (dentalCase.CaseDentalDiseases is not null)
            {
                var existingDentalDiseases = _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases
                    .GetAll(u => u.CaseId == dentalCase.Id, includeProperties: "DentalDiseases").ToList();

                _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases.RemoveRange(existingDentalDiseases);
            }

            if (dentalDiseases is not null)
            {
                foreach (var dentalDiseaseName in dentalDiseases)
                {
                    var dentalDisease = _dentalunitOfWork.DentalCaseRepository.DentalDiseases
                        .Get(u => u.DiseaseName == dentalDiseaseName);

                    if (dentalDisease is not null)
                    {
                        CaseDentalDiseases caseDentalDisease = new CaseDentalDiseases
                        {
                            CaseId = dentalCase.Id,
                            DiseaseId = dentalDisease.Id
                        };

                        _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases.Add(caseDentalDisease);
                    }
                }
            }
        }

        private AuthModel<DentalCaseResponseVM> ValidateCase(DentalCaseRequestVm model)
        {
            var validationErrors = new List<string>();

            if (model.DentalDiseases != null && !model.IsKnown)
            {
                validationErrors.Add("Your dental case must be known to enter your dental diseases");
            }

            if (model.DentalDiseases == null && model.IsKnown)
            {
                validationErrors.Add("Dental diseases must be provided if known");
            }

            int mouthImageCount = model.MouthImages?.Count ?? 0;
            if (mouthImageCount != 0 && (mouthImageCount < 2 || mouthImageCount > 6))
            {
                validationErrors.Add("The number of mouth images should be between 2 and 6 (inclusive)");
            }

            int xrayImageCount = model.XrayImages?.Count ?? 0;
            if (xrayImageCount > 2)
            {
                validationErrors.Add("Maximum Number of XRay Images is 2");
            }

            int prescriptionImageCount = model.PrescriptionImages?.Count ?? 0;
            if (prescriptionImageCount > 2)
            {
                validationErrors.Add("Maximum Number of Prescription Images is 2");
            }

            if (validationErrors.Count > 0)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = string.Join(", ", validationErrors) };
            }

            return new AuthModel<DentalCaseResponseVM> { Success = true };
        }
        
        private DentalCaseResponseVM ConstructDentalCaseResponse(DentalCase dentalCase)
        {
            var DentalDiseases = dentalCase.CaseDentalDiseases.Select(u => u.DentalDiseases.DiseaseName).ToList();
            var ChronicDiseases = dentalCase.CaseChronicDiseases.Select(u => u.ChronicDiseases.DiseaseName).ToList();
            var MouthImages = dentalCase.MouthImages.Select(u => u.ImageLink).ToList();
            var XRayImages = dentalCase.XrayImages.Select(u => u.ImageLink).ToList();
            var PrescriptionImages = dentalCase.PrescriptionImages.Select(u => u.ImageLink).ToList();
            var PatientName = dentalCase.Patient.User.FullName;
            var PatientAge = dentalCase.Patient.User.Age;
            var PatientCity = dentalCase.Patient.User.City;
            var PatientNumber = dentalCase.Patient.User.PhoneNumber;
            var doctorName = dentalCase.Doctor != null ? dentalCase.Doctor.User.FullName : null;
            var doctorUniversity = dentalCase.Doctor != null ? dentalCase.Doctor.University : null;
            var dentalCaseResponse = new DentalCaseResponseVM
            {
                CaseId = dentalCase.Id,
                PatientName = PatientName,
                PatientAge = PatientAge,
                PatientCity = PatientCity,
                DoctorName = doctorName,
                DoctorUniversity = doctorUniversity,
                Description = dentalCase.Description,
                IsKnown = dentalCase.IsKnown,
                IsAssigned = dentalCase.IsAssigned,
                DentalDiseases = DentalDiseases,
                ChronicDiseases = ChronicDiseases,
                MouthImages = MouthImages,
                XrayImages = XRayImages,
                PrescriptionImages = PrescriptionImages,
                PhoneNumber = PatientNumber
            };

            return dentalCaseResponse;
        }
    }

}
