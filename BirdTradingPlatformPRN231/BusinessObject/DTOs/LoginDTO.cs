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
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(255, ErrorMessage = "Email can't be longer than 255 characters")]
        [EmailAddress(ErrorMessage = "Please input a valid Email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or above")]
        [MaxLength(100, ErrorMessage = "Password can't be longer than 100 characters")]
        public string Password { get; set; } = null!;
    }
}
