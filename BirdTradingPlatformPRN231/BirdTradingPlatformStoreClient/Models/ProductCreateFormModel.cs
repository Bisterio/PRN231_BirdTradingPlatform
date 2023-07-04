using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace BirdTradingPlatformStoreClient.Models
{
    public class ProductCreateFormModel
    {
        [Required]
        public string? Description { get; set; } = null!;
        public string? Image { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = null!;
        [Required]
        [Range(1, int.MaxValue)]
        public int Stock { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Required]
        public long? CategoryId { get; set; }
        public long ProductId { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();

        public IFormFile? UploadImage { get; set; }
    }
}
