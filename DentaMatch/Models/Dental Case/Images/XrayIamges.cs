﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models.Dental_Case.Images
{
    public class XrayIamges
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("Case")]
        public string CaseId { get; set; }
        public virtual DentalCase Case { get; set; }
        public string Image { get; set; }
        public string ImageLink { get; set; }

    }
}
