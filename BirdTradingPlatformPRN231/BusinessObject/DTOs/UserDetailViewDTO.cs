﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UserDetailViewDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public byte Status { get; set; }
        public string? StoreName { get; set; }
        public string? StoreAddress { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? LogoImage { get; set; }
        public string? CoverImage { get; set; }
        public string? Description { get; set; }
    }
}
