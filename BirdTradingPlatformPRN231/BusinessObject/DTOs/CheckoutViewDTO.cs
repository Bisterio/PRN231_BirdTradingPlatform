using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class CheckoutViewDTO
    {
        public string? ShippingAddress { get; set; }
        public List<ShippingCalculatedCartItemDTO> CartItems { get; set; } = new List<ShippingCalculatedCartItemDTO>();
    }
}
