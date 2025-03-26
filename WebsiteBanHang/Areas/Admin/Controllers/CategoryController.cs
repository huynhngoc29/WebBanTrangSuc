using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;

public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // GET: Add Category
    public IActionResult Add()
    {
        return View();
    }

    // POST: Add Category
    [HttpPost]
    public async Task<IActionResult> Add(Category category)
    {
        if (ModelState.IsValid)
        {
            await _categoryRepository.AddAsync(category);
            return RedirectToAction("Index");
        }
        return View(category);
    }

    // Hiển thị form cập nhật sản phẩm cho Admin
    // Hiển thị form cập nhật danh mục cho Admin
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // Xử lý cập nhật danh mục
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;

            // Cập nhật danh mục
            await _categoryRepository.UpdateAsync(existingCategory);

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }


    // GET: Delete Category
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

         // Xử lý xóa sản phẩm khi người dùng xác nhận
        [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("DeleteConfirmed")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _categoryRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
    }


    // GET: Category List
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return View(categories);
    }
}
