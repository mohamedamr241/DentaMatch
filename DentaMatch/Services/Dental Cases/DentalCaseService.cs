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


                var existingDentalDiseases = _dentalCaseUnitOfWork.CaseDentalDiseases.GetAll(u => u.CaseId == caseId, includeProperties: "DentalDiseases").ToList();
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

                // Update MouthImages
                //var existingMouthImages = _dentalCaseUnitOfWork.MouthImages.GetAll(u => u.CaseId == caseId).ToList();
                //var mouthImagesToKeep = model.MouthImages
                //    .Where(m => existingMouthImages.Any(em => em.Image == @"\Images\MouthImages\" + m.FileName)).ToList();

                //_dentalCaseUnitOfWork.MouthImages.RemoveRange(existingMouthImages
                //    .Where(em => !mouthImagesToKeep.Any(m => @"\Images\MouthImages\" + m.FileName == em.Image)));

                //foreach (var mouthImage in model.MouthImages.Except(mouthImagesToKeep))
                //{
                //    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(mouthImage.FileName);
                //    string imagePath = @"Images\MouthImages";

                //    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                //    {
                //        mouthImage.CopyTo(fileStream);
                //    }

                //    var newMouthImage = new MouthImages
                //    {
                //        Id = Guid.NewGuid().ToString(),
                //        CaseId = dentalCase.Id,
                //        Image = @"\Images\MouthImages\" + fileName
                //    };

                //    _dentalCaseUnitOfWork.MouthImages.Add(newMouthImage);
                //}

                //// Similar logic for XRayImages and PrescriptionImages
                //// Update XRayImages
                //var existingXRayImages = _dentalCaseUnitOfWork.XRayImages.GetAll(u => u.CaseId == caseId).ToList();
                //var xrayImagesToKeep = model.XrayImages?.Where(x => existingXRayImages.Any(ex => ex.Image == x)).ToList();
                //_dentalCaseUnitOfWork.XRayImages.RemoveRange(existingXRayImages.Where(ex => !xrayImagesToKeep?.Contains(ex.Image) ?? false));

                //foreach (var xrayImage in model.XrayImages?.Except(xrayImagesToKeep ?? Enumerable.Empty<IFormFile>()))
                //{
                //    // Process XrayImage similar to MouthImages
                //}

                //// Update PrescriptionImages
                //var existingPrescriptionImages = _dentalCaseUnitOfWork.PrescriptionImages.GetAll(u => u.CaseId == caseId).ToList();
                //var prescriptionImagesToKeep = model.PrescriptionImages?.Where(p => existingPrescriptionImages.Any(ep => ep.Image == p)).ToList();
                //_dentalCaseUnitOfWork.PrescriptionImages.RemoveRange(existingPrescriptionImages.Where(ep => !prescriptionImagesToKeep?.Contains(ep.Image) ?? false));

                //foreach (var prescriptionImage in model.PrescriptionImages?.Except(prescriptionImagesToKeep ?? Enumerable.Empty<IFormFile>()))
                //{
                //    // Process PrescriptionImage similar to MouthImages
                //}

                //// Save changes
                //_dentalCaseUnitOfWork.Save();

                // Images update logic would be similar:
                // 1. Determine which images to keep.
                // 2. Delete images that are not in the updated list.
                // 3. Add new images.
                // You would use the same pattern as above for MouthImages, XrayImages, and PrescriptionImages.

                _dentalCaseUnitOfWork.DentalCases.Update(dentalCase);
                _dentalCaseUnitOfWork.Save();

                var dentalCaseData = new DentalCaseResponseVM
                {
                    Description = dentalCase.Description,
                    DentalDiseases = model.DentalDiseases.ToList(),
                    ChronicDiseases = model.ChronicDiseases.ToList(),
                    IsKnown = dentalCase.IsKnown,
                    // Populate the paths of the images if they are updated
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


    }
}