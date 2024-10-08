﻿using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Models.Dental_Case.Reports;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models
{
    public class DentalCase
    {
        [Key]
        public string Id { get; set; }
        [Required, StringLength(350, MinimumLength = 20, ErrorMessage = "Minimum Length is 20 and Maximum is 350")]
        public string Description { get; set; }
        [Required]
        public bool IsKnown { get; set; } = false;
        [Required]
        public bool IsAssigned { get; set; } = false;

        [Required]
        public DateTime AppointmentDateTime { get; set; } = new DateTime(9999, 1, 1); 

        public string? GoogleMapLink { get; set; }

        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [ForeignKey("Doctor")]
        public string? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public virtual ICollection<CaseChronicDiseases> CaseChronicDiseases { get; set; }
        public virtual ICollection<CaseDentalDiseases> CaseDentalDiseases { get; set; }
        public virtual ICollection<MouthImages> MouthImages { get; set; }
        public virtual ICollection<XrayIamges> XrayImages { get; set; }
        public virtual ICollection<PrescriptionImages> PrescriptionImages { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
