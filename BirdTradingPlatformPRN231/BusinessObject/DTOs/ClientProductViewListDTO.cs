﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClientProductViewListDTO
    {
        public List<ProductViewDTO?> ProductsPaginated { get; set; } = new List<ProductViewDTO?>();
        public int Page { get; set; }
        public string? Name { get; set; }
        public long Category { get; set; }
        public long Pmin { get; set; }
        public long Pmax { get; set; }
        public int Order { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public List<int>? PageNumbers { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
