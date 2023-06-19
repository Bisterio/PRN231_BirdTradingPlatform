using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(84|0[3|5|7|8|9]|84[3|5|7|8|9])+([0-9]{8})\b$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = null!;

        public long? StoreId { get; set; }

        public byte CreateNewStore { get; set; } = 0; 

        public string? NewStoreName { get; set; }
        public string? NewStoreAddress { get; set; }
    }
}
