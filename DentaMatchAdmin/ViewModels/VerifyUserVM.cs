using DentaMatch.Models;
using DentaMatch.ViewModel.Authentication;
using System.ComponentModel.DataAnnotations;

namespace DentaMatchAdmin.ViewModels
{
    public class VerifyUserVM : BaseVM
    {
        [Required]
        public List<DoctorResponseVM> Doctors { get; set; }
    }
}
