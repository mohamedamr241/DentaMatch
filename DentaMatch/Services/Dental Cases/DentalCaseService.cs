using DentaMatch.Helpers;
using DentaMatch.IServices.Dental_Cases;
using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Repository.Dental_Case;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;


namespace DentaMatch.Services.Dental_Cases
{
    public class DentalCaseService : IDentalCaseService
    {
        private readonly IDentalCaseUnitOfWork _dentalCaseUnitOfWork;
        private readonly DentalCaseHelper _dentalCaseHelper;
        public DentalCaseService(IDentalCaseUnitOfWork dentalCaseUnitOfWork, DentalCaseHelper dentalCaseHelper)
        {
            _dentalCaseUnitOfWork = dentalCaseUnitOfWork;
            _dentalCaseHelper = dentalCaseHelper;
        }
        public AuthModel<DentalCaseResponseVM> CreateCase(string UserId, DentalCaseRequestVm model)
        {
            try
            {
                var patient = _dentalCaseUnitOfWork.Patients.Get(c => c.UserId == UserId);
                if (patient == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "User not found." };
                }

                if (model.DentalDiseases != null && !model.IsKnown && model.DentalDiseases.Count() > 0)
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
                    PatientId = patientId.ToString(),
                    Patient = patient
                };

                _dentalCaseUnitOfWork.DentalCases.Add(dentalCase);
                _dentalCaseUnitOfWork.Save();

                if(model.ChronicDiseases != null)
                {

                var ChronicDiseasesIds = _dentalCaseHelper.GetChronicDiseaseIds(model.ChronicDiseases);
                foreach (var ChronicDiseasesId in ChronicDiseasesIds)
                {
                    var chronicDisease = new CaseChronicDiseases
                    {
                        CaseId = dentalCase.Id,
                        DiseaseId = ChronicDiseasesId
                    };

                    _dentalCaseUnitOfWork.CaseChronicDiseases.Add(chronicDisease);
                }
                }

                if(model.DentalDiseases != null)
                {

                var dentalDiseasesIds = _dentalCaseHelper.GetDentalDiseaseIds(model.DentalDiseases);
                foreach (var dentalDiseasesId in dentalDiseasesIds)
                {
                    var dentalDisease = new CaseDentalDiseases
                    {
                        CaseId = dentalCase.Id,
                        DiseaseId = dentalDiseasesId
                    };

                    _dentalCaseUnitOfWork.CaseDentalDiseases.Add(dentalDisease);
                }
                }

                if (model.MouthImages != null && model.MouthImages.Count() < 2 || model.MouthImages.Count() > 6)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "The number of mouth images should be between 2 and 6, inclusive" };
                }
                if (model.XrayImages != null && model.XrayImages.Count() > 2)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Maximum Number of XRay Images is 2" };
                }
                if (model.PrescriptionImages != null && model.PrescriptionImages.Count() > 2)
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
                        //string ImagePath = @"Images\DentalCase\MouthImages";
                        string ImagePath = Path.Combine("wwwroot", "Images", "DentalCase", "MouthImages");

                        if (!Directory.Exists(ImagePath))
                        {
                            Directory.CreateDirectory(ImagePath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                        {
                            Mouthimage.CopyTo(fileStream);
                        }
                        var mouthImage = new MouthImages
                        {
                            Id = Guid.NewGuid().ToString(),
                            CaseId = dentalCase.Id,
                            Image = @"\Images\DentalCase\MouthImages\" + fileName
                        };
                        _dentalCaseUnitOfWork.MouthImages.Add(mouthImage);
                        MouthImagesPaths.Add(mouthImage.Image);
                    }
                }
                if (model.XrayImages != null)
                {

                foreach (var xrayimage in model.XrayImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(xrayimage.FileName);
                    //string ImagePath = @"Images\DentalCase\XRayImages";
                    string ImagePath = Path.Combine("wwwroot", "Images", "DentalCase", "XRayImages");

                    if (!Directory.Exists(ImagePath))
                    {
                            Directory.CreateDirectory(ImagePath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                    {
                        xrayimage.CopyTo(fileStream);
                    }
                    var xrayImage = new XrayIamges
                    {
                        Id = Guid.NewGuid().ToString(),
                        CaseId = dentalCase.Id,
                        Image = @"\Images\DentalCase\XRayImages\" + fileName
                    };
                    _dentalCaseUnitOfWork.XRayImages.Add(xrayImage);
                    XrayImagesPaths.Add(xrayImage.Image);

                }
                }
                if(model.PrescriptionImages != null)
                {

                foreach (var prescriptionimage in model.PrescriptionImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(prescriptionimage.FileName);
                    //string ImagePath = @"Images\DentalCase\PrescriptionImages";
                    string ImagePath = Path.Combine("wwwroot", "Images", "DentalCase", "PrescriptionImages");

                    if (!Directory.Exists(ImagePath))
                    {
                         Directory.CreateDirectory(ImagePath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                    {
                        prescriptionimage.CopyTo(fileStream);
                    }
                    var prescriptionImage = new PrescriptionImages
                    {
                        Id = Guid.NewGuid().ToString(),
                        CaseId = dentalCase.Id,
                        Image = @"\Images\DentalCase\PrescriptionImages\" + fileName
                    };
                    _dentalCaseUnitOfWork.PrescriptionImages.Add(prescriptionImage);
                    PrescriptionImagesPaths.Add(prescriptionImage.Image);

                }
                }
                var dentalCaseData = new DentalCaseResponseVM
                {
                    Description = model.Description,
                    DentalDiseases = model.DentalDiseases,
                    ChronicDiseases = model.ChronicDiseases,
                    IsKnown = model.IsKnown,
                    MouthImages = MouthImagesPaths,
                    PrescriptionImages = PrescriptionImagesPaths,
                    XrayImages = XrayImagesPaths
                };
                _dentalCaseUnitOfWork.Save();

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

        public AuthModel DeleteCase(string caseId)
        {
            try
            {
                var dentalCase = _dentalCaseUnitOfWork.DentalCases.Get(u => u.Id == caseId);
                if (dentalCase == null)
                {
                    return new AuthModel { Success = false, Message = "Dental Case Not Found" };
                }
                var mouthImagesPaths = _dentalCaseUnitOfWork.MouthImages.GetAll(u => u.CaseId == caseId).Select(u => u.Image);
                var XRayImagesPaths = _dentalCaseUnitOfWork.XRayImages.GetAll(u => u.CaseId == caseId).Select(u => u.Image);
                var PrescriptionImages = _dentalCaseUnitOfWork.PrescriptionImages.GetAll(u => u.CaseId == caseId).Select(u => u.Image);
                _dentalCaseHelper.DeleteImages(mouthImagesPaths);
                _dentalCaseHelper.DeleteImages(XRayImagesPaths);
                _dentalCaseHelper.DeleteImages(PrescriptionImages);

                _dentalCaseUnitOfWork.DentalCases.Remove(dentalCase);
                _dentalCaseUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "Dental Case deleted successfully" };
            }
            catch(Exception error) 
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error Deleting dental case: {error.Message}" };
            }
        }

        

        public AuthModel<DentalCaseResponseVM> UpdateCase(string caseId, DentalCaseRequestVm model)
        {
            try
            {
                var dentalCase = _dentalCaseUnitOfWork.DentalCases.Get(u => u.Id == caseId, "CaseChronicDiseases,CaseDentalDiseases,MouthImages,XrayImages,PrescriptionImages");
                if (dentalCase == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental Case Not Found" };
                }

                if (model.DentalDiseases != null && !model.IsKnown && model.DentalDiseases.Count() > 0)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Your dental case must be known to enter your dental diseases" };
                }

                if (model.DentalDiseases == null && model.IsKnown)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental diseases must be provided" };
                }
                dentalCase.Description = model.Description;
                dentalCase.IsKnown = model.IsKnown;


                var existingChronicDiseases = _dentalCaseUnitOfWork.CaseChronicDiseases.GetAll(u => u.CaseId == caseId, includeProperties: "ChronicDiseases").ToList();
                if(model.ChronicDiseases == null)
                {
                    _dentalCaseUnitOfWork.CaseChronicDiseases.RemoveRange(existingChronicDiseases);
                }
                else
                {
                    var chronicDiseasesToKeep = model.ChronicDiseases
                            .Where(diseaseName => existingChronicDiseases.Any(ed => ed.ChronicDiseases.DiseaseName == diseaseName))
                            .ToList();
                    _dentalCaseUnitOfWork.CaseChronicDiseases.RemoveRange(existingChronicDiseases.Where(ed => !chronicDiseasesToKeep.Contains(ed.ChronicDiseases.DiseaseName)));

                    foreach (var chronicDiseaseName in model.ChronicDiseases.Except(chronicDiseasesToKeep))
                    {
                            var chronicdiseaseId = _dentalCaseUnitOfWork.ChronicDiseases.Get(u => u.DiseaseName == chronicDiseaseName).Id;
                            var chronicDisease = new CaseChronicDiseases
                            {
                                CaseId = dentalCase.Id,
                                DiseaseId = chronicdiseaseId.ToString()
                            };
                         _dentalCaseUnitOfWork.CaseChronicDiseases.Add(chronicDisease);
                    }
                }
                var existingDentalDiseases = _dentalCaseUnitOfWork.CaseDentalDiseases.GetAll(u => u.CaseId == caseId, includeProperties: "DentalDiseases").ToList();
                if(model.DentalDiseases == null)
                {
                    _dentalCaseUnitOfWork.CaseDentalDiseases.RemoveRange(existingDentalDiseases);
                }
                else
                {
                    var dentalDiseasesToKeep = model.DentalDiseases
                        .Where(diseaseName => existingDentalDiseases.Any(ed => ed.DentalDiseases.DiseaseName == diseaseName))
                        .ToList();
                    _dentalCaseUnitOfWork.CaseDentalDiseases.RemoveRange(existingDentalDiseases.Where(ed => !dentalDiseasesToKeep.Contains(ed.DentalDiseases.DiseaseName)));

                    foreach (var dentalDiseaseName in model.DentalDiseases.Except(dentalDiseasesToKeep))
                    {
                        var DentaldiseaseId = _dentalCaseUnitOfWork.DentalDiseases.Get(u => u.DiseaseName == dentalDiseaseName).Id;
                        var dentalDisease = new CaseDentalDiseases
                        {
                            CaseId = dentalCase.Id,
                            DiseaseId = DentaldiseaseId.ToString()
                        };
                        _dentalCaseUnitOfWork.CaseDentalDiseases.Add(dentalDisease);
                    }
                }
                

                _dentalCaseUnitOfWork.DentalCases.Update(dentalCase);
                _dentalCaseUnitOfWork.Save();

                var dentalCaseData = new DentalCaseResponseVM
                {
                    Description = dentalCase.Description,
                    DentalDiseases = model.DentalDiseases,
                    ChronicDiseases = model.ChronicDiseases,
                    IsKnown = dentalCase.IsKnown,
                };

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
        public AuthModel<DentalCaseResponseVM> GetCase(string caseId)
        {
            try
            {
                var dentalCase = _dentalCaseUnitOfWork.DentalCases.Get(u => u.Id == caseId, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
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
            catch(Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error Retrieving dental case: {error.Message}" };
            }
        }

        public AuthModel<List<DentalCaseResponseVM>> GetPatientCases(string UserId)
        {
            try
            {
                var patient = _dentalCaseUnitOfWork.Patients.Get(c => c.UserId == UserId);
                if (patient == null)
                {
                    return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = "User not found." };
                }
                var DentalCases = _dentalCaseUnitOfWork.DentalCases.GetAll((u => u.PatientId == patient.Id), "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
                if(DentalCases.Count() != 0)
                {
                    List< DentalCaseResponseVM> PatientDentalCases = new List<DentalCaseResponseVM>();
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
            catch(Exception error)
            {
                return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = $"Error Retrieving dental case: {error.Message}" };
            }
        }

        public AuthModel<List<DentalCaseResponseVM>> GetUnAssignedCases()
        {
            try
            {
                var DentalCases = _dentalCaseUnitOfWork.DentalCases.GetAll((u => !u.IsAssigned), 
                    "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
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

        public AuthModel<List<DentalCaseResponseVM>> GetAssignedCases(string UserId)
        {
            try
            {
                var doctor = _dentalCaseUnitOfWork.Doctors.Get(c => c.UserId == UserId);
                if (doctor == null)
                {
                    return new AuthModel<List<DentalCaseResponseVM>> { Success = false, Message = "User not found" };
                }
                var DentalCases = _dentalCaseUnitOfWork.DentalCases.GetAll((u => u.DoctorId == doctor.Id),
                    "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages,Patient.User");
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
                var ChronicDiseases = Dentalcase.CaseChronicDiseases.Select(u => u.ChronicDiseases.DiseaseName).ToList();
                var DentalDiseases = Dentalcase.CaseDentalDiseases.Select(u => u.DentalDiseases.DiseaseName).ToList();
                var MouthImages = Dentalcase.MouthImages.Select(u => u.Image).ToList();
                var XRayImages = Dentalcase.XrayImages.Select(u => u.Image).ToList();
                var PrescriptionImages = Dentalcase.PrescriptionImages.Select(u => u.Image).ToList();
                var PatientName = Dentalcase.Patient.User.FullName;
                var PatientAge = Dentalcase.Patient.User.Age;
                var PatientCity = Dentalcase.Patient.User.City;
                var dentalCaseData = new DentalCaseResponseVM
                {
                    CaseId = Dentalcase.Id,
                    PatientName = PatientName,
                    PatientAge = PatientAge,
                    PatientCity = PatientCity,
                    Description = Dentalcase.Description,
                    DentalDiseases = DentalDiseases,
                    ChronicDiseases = ChronicDiseases,
                    IsKnown = Dentalcase.IsKnown,
                    IsAssigned = Dentalcase.IsAssigned,
                    PrescriptionImages = PrescriptionImages,
                    XrayImages = XRayImages,
                    MouthImages = MouthImages,
                };

            
            return dentalCaseData;
        }
    }
}