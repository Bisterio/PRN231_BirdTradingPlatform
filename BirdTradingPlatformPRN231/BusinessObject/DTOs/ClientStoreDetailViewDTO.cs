using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClientStoreDetailViewDTO
    {
        public long StoreId { get; set; }
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoImage { get; set; }
        public string? CoverImage { get; set; }
        public List<ProductViewDTO?> ProductList { get; set; } = new List<ProductViewDTO?>();
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
