using DentaMatch.Data;
using DentaMatch.Helpers;
using DentaMatch.IServices.Dental_Cases;
using DentaMatch.Models;
using DentaMatch.Models.Patient_Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Images;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.CodeAnalysis.Operations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Services.Dental_Cases
{
    public class DentalCaseService : IDentalCaseService<DentalCaseResponseVM>
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
                var user = _dentalCaseUnitOfWork.Patients.Get(c => c.UserId == UserId);
                if (user == null)
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
                string patientId = user.Id;
                var dentalCase = new DentalCase
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = model.Description,
                    IsKnown = model.IsKnown,
                    PatientId = patientId.ToString()
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
                        _dentalCaseUnitOfWork.MouthImages.Add(mouthImage);
                        MouthImagesPaths.Add(mouthImage.Image);
                    }
                }
                if (model.XrayImages != null)
                {

                foreach (var xrayimage in model.XrayImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(xrayimage.FileName);
                    string ImagePath = @"Images\XRayImages";
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
                    _dentalCaseUnitOfWork.XRayImages.Add(xrayImage);
                    XrayImagesPaths.Add(xrayImage.Image);

                }
                }
                if(model.PrescriptionImages != null)
                {

                foreach (var prescriptionimage in model.PrescriptionImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(prescriptionimage.FileName);
                    string ImagePath = @"Images\PrescriptionImages";      
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
                var dentalCase = _dentalCaseUnitOfWork.DentalCases.Get(u => u.Id == caseId, "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages");
                if (dentalCase == null)
                {
                    return new AuthModel<DentalCaseResponseVM> { Success = false, Message = "Dental Case Not Found" };
                }
                var ChronicDiseases = dentalCase.CaseChronicDiseases.Select(u => u.ChronicDiseases.DiseaseName).ToList();
                var DentalDiseases = dentalCase.CaseDentalDiseases.Select(u => u.DentalDiseases.DiseaseName).ToList();
                var MouthImages = dentalCase.MouthImages.Select(u => u.Image).ToList();
                var XRayImages = dentalCase.XrayImages.Select(u => u.Image).ToList();
                var PrescriptionImages = dentalCase.PrescriptionImages.Select(u => u.Image).ToList();
                var dentalCaseData = new DentalCaseResponseVM
                {
                    Id = dentalCase.Id,
                    PatientId = dentalCase.PatientId,
                    Description = dentalCase.Description,
                    DentalDiseases = DentalDiseases,
                    ChronicDiseases = ChronicDiseases,
                    IsKnown = dentalCase.IsKnown,
                    PrescriptionImages= PrescriptionImages,
                    XrayImages= XRayImages,
                    MouthImages= MouthImages,
               };
                return new AuthModel<DentalCaseResponseVM>
                {
                    Success = true,
                    Message = "Dental Case retrieved successfully",
                    Data = dentalCaseData
                };
            }
            catch(Exception error)
            {
                return new AuthModel<DentalCaseResponseVM> { Success = false, Message = $"Error Retrieving dental case: {error.Message}" };
            }
        }

        public AuthModel<IEnumerable<DentalCase>> GetAllCase(string UserId)
        {
            try
            {
                var user = _dentalCaseUnitOfWork.Patients.Get(c => c.UserId == UserId);
                if (user == null)
                {
                    return new AuthModel<IEnumerable<DentalCase>> { Success = false, Message = "User not found." };
                }
                var AllPatientCases = _dentalCaseUnitOfWork.DentalCases.GetAll((u => u.PatientId == user.Id), "CaseChronicDiseases.ChronicDiseases,CaseDentalDiseases.DentalDiseases,MouthImages,XrayImages,PrescriptionImages");
                return new AuthModel<IEnumerable<DentalCase>>
                {
                    Success = true,
                    Message = "Dental Cases retrieved successfully",
                    Data = AllPatientCases
                };
            }
            catch(Exception error)
            {
                return new AuthModel<IEnumerable<DentalCase>> { Success = false, Message = $"Error Retrieving dental case: {error.Message}" };
            }
        }
    }
}