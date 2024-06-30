using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Notifications
{
    public class FireBaseVM
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string userToken { get; set; }
    }
}
