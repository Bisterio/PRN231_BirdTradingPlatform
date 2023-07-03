using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UserPasswordUpdateDTO
    {
        [Required(ErrorMessage = "Old Password is required")]
        [MinLength(8, ErrorMessage = "Old Password must be over 8 characters")]
        [MaxLength(100, ErrorMessage = "Old Password must be under 100 characters")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        [MinLength(8, ErrorMessage = "New Password must be over 8 characters")]
        [MaxLength(100, ErrorMessage = "New Password must be under 100 characters")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [MinLength(8, ErrorMessage = "Confirm Password must be over 8 characters")]
        [MaxLength(100, ErrorMessage = "Confirm Password must be under 100 characters")]
        public string ConfirmPassword { get; set; }
    }
}
