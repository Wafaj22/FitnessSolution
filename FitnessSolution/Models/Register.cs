using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FitnessSolution.Models
{
    public class Register
    {
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Birth date")]
        public DateTime DOB { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
