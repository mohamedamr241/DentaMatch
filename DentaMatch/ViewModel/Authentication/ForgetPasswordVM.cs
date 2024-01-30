﻿using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class ForgetPasswordVM
    {
        [Required]
        public string Email { get; set; }
        [MaxLength(5)]
        public string VerificationCode { get; set; }
    }
}
