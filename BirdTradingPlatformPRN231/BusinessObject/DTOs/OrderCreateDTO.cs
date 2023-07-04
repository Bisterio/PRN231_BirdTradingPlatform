using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class OrderCreateDTO
    {
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string PaymentMethod { get; set; } = null!;
        [Required]
        [RegularExpression(@"^(\+84|84|0[1-9]|84[1-9]|\+84[1-9])+([0-9]{8})\b$", ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }
        public decimal TotalShippingCost { get; set; }
        public string? Note { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    }
}
