﻿using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Patient_Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Patient_Models.Dental_Case.Dental_Diseases;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

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

            var patientId = _db.Patients.FirstOrDefault(c => c.UserId == UserId);

            var dentalCase = new DentalCase
            {
                Id = Guid.NewGuid().ToString(),
                Description = model.Description,
                IsAssigned = model.IsAssigned,
                IsKnown = model.IsKnown,
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

            _db.SaveChanges();

            var dentalCaseData = new DentalCaseVm
            {
                Description = model.Description,
                IsAssigned = model.IsAssigned,
                IsKnown = model.IsKnown,
                DentalDiseases = model.DentalDiseases.ToList(),
                ChronicDiseases = model.ChronicDiseases.ToList(),
                MouthImages = model.MouthImages.ToList(),
                PrescriptionImages = model.PrescriptionImages.ToList(),
                XrayImages = model.XrayImages.ToList()
            };

            return new AuthModel<DentalCaseVm>
            {
                Success = true,
                Message = "Dental Case Created successfully",
                Data  = dentalCaseData
            };
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
            return _db.ChronicDiseases
                .Where(cd => dentalDiseaseNames.Contains(cd.DiseaseName))
                .Select(cd => cd.Id)
                .ToList();
        }
    }
}