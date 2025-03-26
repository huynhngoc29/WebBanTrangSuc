using System.ComponentModel.DataAnnotations;

namespace WebBanTrangSuc.Models
{
    public class CategorySubCategory
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public int SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }
    }

}
