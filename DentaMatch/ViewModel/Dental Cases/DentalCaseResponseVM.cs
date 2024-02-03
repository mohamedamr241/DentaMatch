﻿using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Dental_Cases
{
    public class DentalCaseResponseVM
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsKnown { get; set; }
        [Required]
        public bool IsAssigned { get; set; }
        [Required]
        public List<string> MouthImages { get; set; }
        [Required]
        public List<string> XrayImages { get; set; }
        [Required]
        public List<string> PrescriptionImages { get; set; }
        [Required]
        public List<string> DentalDiseases { get; set; }
        [Required]
        public List<string> ChronicDiseases { get; set; }
    }
}
