using System.ComponentModel.DataAnnotations;

namespace WebBanTrangSuc.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }

        public List<CategorySubCategory>? CategorySubCategories { get; set; }
    }
}
