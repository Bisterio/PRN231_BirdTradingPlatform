using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UserProfileUpdateDTO
    {
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
