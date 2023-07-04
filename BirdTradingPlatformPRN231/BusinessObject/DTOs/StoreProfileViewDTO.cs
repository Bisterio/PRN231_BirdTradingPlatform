using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class StoreProfileViewDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StoreDescription { get; set; }
        public string StoreLogoImage { get; set; }
        public string StoreCoverImage { get; set; }
    }
}
