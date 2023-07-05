using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClientUserViewListDTO
    {
        public List<UserDetailViewDTO?> UsersPaginated { get; set; } = new List<UserDetailViewDTO?>();
        public int Page { get; set; }
        public string? RoleSearch { get; set; }
        public long Pmin { get; set; }
        public long Pmax { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public List<int>? PageNumbers { get; set; }
    }
}
