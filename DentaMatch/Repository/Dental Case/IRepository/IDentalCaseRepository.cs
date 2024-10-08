﻿using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Models.Dental_Case.Reports;
using DentaMatch.Repository;
using DentaMatch.ViewModel.Dental_Cases;
using System.Linq.Expressions;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalCaseRepository : IRepository<DentalCase>
    {
        IRepository<DentalDisease> DentalDiseases { get; }
        IRepository<CaseDentalDiseases> CaseDentalDiseases { get; }
        IRepository<CaseChronicDiseases> CaseChronicDiseases { get; }
        IRepository<ChronicDisease> ChronicDiseases { get; }
        IRepository<MouthImages> MouthImages { get; }
        void updateDoctorRequestStatus(DentalCase request);
        IRepository<XrayIamges> XRayImages { get; }
        IRepository<PrescriptionImages> PrescriptionImages { get; }
        IRepository<Report> Report { get; }
        void UpdateDentalCaseProperties(DentalCase dentalCase, bool isKnown, string? description = null, DateTime? appointmentDateTime = null, string? googleMapLink = null);
        IEnumerable<DentalCase> FullTextSearch(Expression<Func<DentalCase, bool>> filter, string searchText, string? includeProperties = null);
        IEnumerable<DentalCase> GetFirstThreeCases(Expression<Func<DentalCase, bool>> filter = null, string? includeProperties = null);
    }
}
