using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class CartAddressDTO
    {
        public string ShippingAddress { get; set; } = null!;
        public List<OrderItemCartDTO> CartItems { get; set; } = new List<OrderItemCartDTO>();
    }
}
