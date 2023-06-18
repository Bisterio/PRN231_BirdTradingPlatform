using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public long CategoryId { get; set; }
        public string Description { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
