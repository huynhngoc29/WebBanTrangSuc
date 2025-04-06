using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanTrangSuc.Models;

public class CategoryMenuViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CategoryMenuViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.CategorySubCategories)
                .ThenInclude(cs => cs.SubCategory)
            .ToListAsync();

        // Thêm mục "Tất cả sản phẩm"
        categories.Insert(0, new Category
        {
            Id = 0,
            CategorySubCategories = new List<CategorySubCategory>()
        });

        return View(categories);
    }

}

