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

                if (model.DentalDiseases != null && !model.IsKnown)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Your dental case must be known to enter your dental diseases" };
                }

                if (model.DentalDiseases == null && model.IsKnown)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental diseases must be provided" };
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
                _dentalunitOfWork.Save();


                if (model.DentalDiseases != null)
                {
                    foreach (var diseaseName in model.DentalDiseases)
                    {
                        var dentalDisease = _dentalunitOfWork.DentalCaseRepository.DentalDiseases.Get(u => u.DiseaseName == diseaseName);

                        if (dentalDisease != null)
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

                if (model.ChronicDiseases != null)
                {
                    foreach (var diseaseName in model.ChronicDiseases)
                    {
                        var ChronicDisease = _dentalunitOfWork.DentalCaseRepository.ChronicDiseases.Get(u => u.DiseaseName == diseaseName);

                        if (ChronicDisease != null)
                        {
                            CaseChronicDiseases caseChronicDisease = new CaseChronicDiseases
                            {
                                CaseId = dentalCase.Id,
                                DiseaseId = ChronicDisease.Id
                            };

                            _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.Add(caseChronicDisease);
                        }
                    }
                }

                if (model.MouthImages != null)
                {
                    int mouthImageCount = model.MouthImages.Count;
                    if (mouthImageCount < 2 || mouthImageCount > 6)
                    {
                        return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "The number of mouth images should be between 2 and 6, inclusive" };
                    }

                    string MouthImagesPath = Path.Combine("wwwroot", "Images", "DentalCase", "MouthImages");
                    foreach (var mouthImage in model.MouthImages)
                    {
                        string ImageName = _appHelper.SaveImage(mouthImage, MouthImagesPath);

                        var newMouthImage = new MouthImages
                        {
                            Id = Guid.NewGuid().ToString(),
                            CaseId = dentalCase.Id,
                            Image = Path.Combine(MouthImagesPath,ImageName),
                            ImageLink = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "DentalCase", "MouthImages", ImageName)
                    };
                        _dentalunitOfWork.DentalCaseRepository.MouthImages.Add(newMouthImage);
                    }
                }

                if (model.XrayImages != null)
                {
                    int xrayImageCount = model.XrayImages.Count;
                    if (xrayImageCount > 2)
                    {
                        return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Maximum Number of XRay Images is 2" };
                    }

                    string XrayImagesPath = Path.Combine("wwwroot", "Images", "DentalCase", "XRayImages");
                    foreach (var xrayImage in model.XrayImages)
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

                if (model.PrescriptionImages != null)
                {
                    int prescriptionImageCount = model.PrescriptionImages.Count;
                    if (prescriptionImageCount > 2)
                    {
                        return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Maximum Number of Prescription Images is 2" };
                    }

                    string PrescriptionImagesPath = Path.Combine("wwwroot", "Images", "DentalCase", "PrescriptionImages");
                    foreach (var prescriptionImage in model.PrescriptionImages)
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
                _dentalunitOfWork.Save();

                var dentalCaseObj = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == dentalCase.Id, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
                var dentalCaseData = ConstructDentalCaseResponse(dentalCaseObj);
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

        public AuthModel<DentalCaseResponseVM> UpdateCase(string caseId, DentalCaseUpdateVM model)
        {
            try
            {
                var dentalCase = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId, "CaseChronicDiseases,CaseDentalDiseases,MouthImages,XrayImages,PrescriptionImages");
                if (dentalCase == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental Case Not Found" };
                }

                if (model.DentalDiseases != null && !model.IsKnown)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Your dental case must be known to enter your dental diseases" };
                }

                if (model.DentalDiseases == null && model.IsKnown)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental diseases must be provided" };
                }
                dentalCase.Description = model.Description;
                dentalCase.IsKnown = model.IsKnown;


                var existingChronicDiseases = _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.GetAll(u => u.CaseId == caseId, includeProperties: "ChronicDiseases").ToList();
                if (model.ChronicDiseases == null || model.ChronicDiseases[0]=="None")
                {
                    _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.RemoveRange(existingChronicDiseases);
                }
                else
                {
                    var chronicDiseasesToKeep = model.ChronicDiseases
                            .Where(diseaseName => existingChronicDiseases.Any(ed => ed.ChronicDiseases.DiseaseName == diseaseName))
                            .ToList();
                    _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.RemoveRange(existingChronicDiseases.Where(ed => !chronicDiseasesToKeep.Contains(ed.ChronicDiseases.DiseaseName)));

                    foreach (var chronicDiseaseName in model.ChronicDiseases.Except(chronicDiseasesToKeep))
                    {
                        var chronicdiseaseId = _dentalunitOfWork.DentalCaseRepository.ChronicDiseases.Get(u => u.DiseaseName == chronicDiseaseName).Id;
                        var chronicDisease = new CaseChronicDiseases
                        {
                            CaseId = dentalCase.Id,
                            DiseaseId = chronicdiseaseId.ToString()
                        };
                        _dentalunitOfWork.DentalCaseRepository.CaseChronicDiseases.Add(chronicDisease);
                    }
                }

                var existingDentalDiseases = _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases.GetAll(u => u.CaseId == caseId, includeProperties: "DentalDiseases").ToList();
                if (model.DentalDiseases == null || model.ChronicDiseases[0] == "None")
                {
                    _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases.RemoveRange(existingDentalDiseases);
                }
                else
                {
                    var dentalDiseasesToKeep = model.DentalDiseases
                        .Where(diseaseName => existingDentalDiseases.Any(ed => ed.DentalDiseases.DiseaseName == diseaseName))
                        .ToList();
                    _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases.RemoveRange(existingDentalDiseases.Where(ed => !dentalDiseasesToKeep.Contains(ed.DentalDiseases.DiseaseName)));

                    foreach (var dentalDiseaseName in model.DentalDiseases.Except(dentalDiseasesToKeep))
                    {
                        var DentaldiseaseId = _dentalunitOfWork.DentalCaseRepository.DentalDiseases.Get(u => u.DiseaseName == dentalDiseaseName).Id;
                        var dentalDisease = new CaseDentalDiseases
                        {
                            CaseId = dentalCase.Id,
                            DiseaseId = DentaldiseaseId.ToString()
                        };
                        _dentalunitOfWork.DentalCaseRepository.CaseDentalDiseases.Add(dentalDisease);
                    }
                }


                _dentalunitOfWork.DentalCaseRepository.Update(dentalCase);
                _dentalunitOfWork.Save();

                var dentalCaseObj = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == dentalCase.Id, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
                var dentalCaseData = ConstructDentalCaseResponse(dentalCaseObj);
                return new AuthModel<DentalCaseResponseVM>
                {
                    Success = true,
                    Message = "Dental Case updated successfully",
                    Data = dentalCaseData
                };
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

        public AuthModel<List<DentalCaseResponseVM>> GetUnAssignedCases()
        {
            try
            {
                var DentalCases = _dentalunitOfWork.DentalCaseRepository.GetAll((u => !u.IsAssigned),
                    "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User");
                if (DentalCases.Count() != 0)
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
        public AuthModel<List<DentalCaseResponseVM>> SearchByDentalDisease(string diseaseName)
        {
            try
            {
                var cases = _dentalunitOfWork.DentalCaseRepository.GetAll(
                    filter: c => c.CaseDentalDiseases.Any(cd => cd.DentalDiseases.DiseaseName == diseaseName),
                    includeProperties: "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User,Doctor.User"
                );
                
                if (cases!=null && cases.Count() != 0)
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



        private DentalCaseResponseVM ConstructDentalCaseResponse(DentalCase Dentalcase)
        {
            var DentalDiseases = Dentalcase.CaseDentalDiseases.Select(u => u.DentalDiseases.DiseaseName).ToList();
            var ChronicDiseases = Dentalcase.CaseChronicDiseases.Select(u => u.ChronicDiseases.DiseaseName).ToList();
            var MouthImages = Dentalcase.MouthImages.Select(u => u.ImageLink).ToList();
            var XRayImages = Dentalcase.XrayImages.Select(u => u.ImageLink).ToList();
            var PrescriptionImages = Dentalcase.PrescriptionImages.Select(u => u.ImageLink).ToList();
            var PatientName = Dentalcase.Patient.User.FullName;
            var PatientAge = Dentalcase.Patient.User.Age;
            var PatientCity = Dentalcase.Patient.User.City;
            var PatientNumber = Dentalcase.Patient.User.PhoneNumber;
            var DoctorName = "";
            var DoctorUniversity = "";
            if (Dentalcase.Doctor != null)
            {
                DoctorName = Dentalcase.Doctor.User.FullName;
                DoctorUniversity = Dentalcase.Doctor.University;
            }
            var dentalCaseResponse = new DentalCaseResponseVM
            {
                CaseId = Dentalcase.Id,
                PatientName = PatientName,
                PatientAge = PatientAge,
                PatientCity = PatientCity,
                DoctorName = DoctorName,
                DoctorUniversity = DoctorUniversity,
                Description = Dentalcase.Description,
                IsKnown = Dentalcase.IsKnown,
                IsAssigned = Dentalcase.IsAssigned,
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
