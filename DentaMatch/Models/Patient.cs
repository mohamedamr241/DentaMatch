using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentaMatch.Models
{
    public class Patient 
    {

        [Key]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string ChronicDiseases { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }
}
