﻿using DentaMatch.ViewModel.Authentication.Response;

namespace DentaMatch.ViewModel.Authentication
{
    public class DoctorResponseVM : UserResponseVM
    {
        public string doctorId { get; set; }
        public string University { get; set; }
        public string CardImage { get; set; }
        public string CardImageLink { get; set; }
        public string Specialization { get; set; }

    }
}
