using System.ComponentModel.DataAnnotations;

namespace BirdTradingPlatformStoreClient.Models
{
    public class StoreCreateFormModel 
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address")]
        [MaxLength(255, ErrorMessage = "Email cannot be over 255 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be over 100 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be over 8 characters")]
        [MaxLength(100, ErrorMessage = "Password must be under 100 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm Password is required")]
        [MinLength(8, ErrorMessage = "Confirm Password must be over 8 characters")]
        [MaxLength(100, ErrorMessage = "Confirm Password must be under 100 characters")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\+84|84|0[1-9]|84[1-9]|\+84[1-9])+([0-9]{8})\b$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = null!;


        [Required(ErrorMessage = "Store Name is required")]
        [MaxLength(255, ErrorMessage = "Store Name cannot be over 255 characters")]
        public string? NewStoreName { get; set; }

        [Required(ErrorMessage = "Store Address is required")]
        [MaxLength(255, ErrorMessage = "Store Address cannot be over 255 characters")]
        public string? NewStoreAddress { get; set; }

        [Required(ErrorMessage = "Store Description is required")]
        [MaxLength(2500, ErrorMessage = "Store Description cannot be over 2500 characters")]
        public string? NewStoreDescription { get; set; }

        public string? NewStoreLogoImage { get; set; }
        public string? NewStoreCoverImage { get; set; }

        public IFormFile? UploadLogoImage { get; set; }
        public IFormFile? UploadCoverImage { get; set; }
    }
}
