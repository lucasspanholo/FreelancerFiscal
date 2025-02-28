using FreelancerFiscal.Domain.Entities;
using FreelancerFiscal.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelancerFiscal.Application.DTOs
{
    // RegisterUserDto
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string Document { get; set; }
        public string Phone { get; set; }
        public string TaxRegime { get; set; }

        // Add other necessary registration fields
    }
}
