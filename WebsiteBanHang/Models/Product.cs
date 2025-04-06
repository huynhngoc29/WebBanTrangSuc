using System.ComponentModel.DataAnnotations;

namespace WebBanTrangSuc.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.0, 10000000000.0)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }

        public Category? Category { get; set; }

        public SubCategory? SubCategory { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? ImageUrls { get; set; }
        // Mặc định là không có giảm giá (0%) và không nằm trong chương trình giảm giá
        public decimal DiscountPercentage { get; set; } = 0; // Không giảm giá
        public bool IsOnSale { get; set; } = false;  // Không giảm giá

        public int Quantity { get; set; } = 0;
    }
}
