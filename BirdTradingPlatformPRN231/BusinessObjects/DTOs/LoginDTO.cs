using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessObject.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public string? RefreshToken { get; set; }
    }
}
