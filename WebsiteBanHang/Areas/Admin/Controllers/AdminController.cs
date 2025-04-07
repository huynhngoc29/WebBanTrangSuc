using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanTrangSuc.Areas.Admin.Models;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace WebBanTrangSuc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    [Route("Admin/")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly ISubCategoryRepository _subCategoryRepository;


        public AdminController(IProductRepository productRepository,
       ICategoryRepository categoryRepository, ApplicationDbContext context, ISubCategoryRepository subCategoryRepository) // 👉 thêm vào đây
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository; // 👉 gán vào đây
        }

        // Route cho trang chủ
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        // Route cho danh sách sản phẩm
        [Route("Products")]
        public async Task<IActionResult> Products(int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetAllAsync();
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            return View(pagedProducts);
        }

        // Lọc sản phẩm, tìm kiếm và sắp xếp
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index(string search, string sortOrder, int? categoryId, int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetAllAsync();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
            }

            // Xử lý phân trang
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Truyền thông tin phân trang vào ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View(pagedProducts);
        }


        // Hiển thị form thêm sản phẩm mới cho Admin
        [Authorize(Roles = "Admin")]
        // Đảm bảo danh mục được truyền qua ViewBag như sau
        [Route("Product/Add")]
        //public async Task<IActionResult> Add()
        //{
        //    var categories = await _categoryRepository.GetAllAsync();
        //    var subCategories = await _subCategoryRepository.GetAllAsync();
        //    var validCategories = categories.Where(c => c.Name != null).ToList(); // Lọc bỏ danh mục NULL
        //    ViewBag.Categories = new SelectList(validCategories, "Id", "Name");
        //    ViewBag.SubCategories = new SelectList(subCategories, "Id", "Name");// Chuyển danh sách sang SelectList

        //    return View();
        //}
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var subCategories = await _subCategoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.SubCategories = new SelectList(subCategories, "Id", "Name");
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Product/Add")]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                await _productRepository.AddAsync(product);

                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                return RedirectToAction("Products", "Admin", new { area = "Admin" });
            }

            // Load lại Category + SubCategory khi lỗi
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            var subCategories = await _context.CategorySubCategories
                .Where(cs => cs.CategoryId == product.CategoryId)
                .Include(cs => cs.SubCategory)
                .Select(cs => cs.SubCategory!)
                .ToListAsync();

            ViewBag.SubCategories = new SelectList(subCategories, "Id", "Name", product.SubCategoryId);

            return View(product);
        }

        [HttpGet]
        [Route("Admin/GetSubCategoriesByCategory")]
        public async Task<IActionResult> GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = await _context.CategorySubCategories
                .Where(cs => cs.CategoryId == categoryId)
                .Include(cs => cs.SubCategory)
                .Select(cs => new
                {
                    value = cs.SubCategory!.Id,
                    text = cs.SubCategory.Name
                })
                .ToListAsync();

            return Json(subCategories);
        }




        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }
        // Hiển thị thông tin sản phẩm
        [Route("Product/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        [Route("Product/Update/{id}")]

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdWithCategoryAndVariantsAsync(id); // ✅ cần include Variants
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            var subCategories = await _subCategoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.SubCategories = new SelectList(subCategories, "Id", "Name");
            return View(product);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Product/Update/{id}")]

        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho trường ImageUrl

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id); // Lấy sản phẩm hiện tại từ DB

                // Kiểm tra nếu không có ảnh mới, giữ nguyên ảnh cũ
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu ảnh mới nếu có
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.Quantity = product.Quantity;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.SubCategoryId = product.SubCategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction("Products", "Admin", new { area = "Admin" }); // Quay lại danh sách sản phẩm
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form và các danh mục
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var subCategories = await _context.CategorySubCategories
                .Where(cs => cs.CategoryId == product.CategoryId)
                .Include(cs => cs.SubCategory)
                .Select(cs => cs.SubCategory!)
                .ToListAsync();

            ViewBag.SubCategories = new SelectList(subCategories, "Id", "Name", product.SubCategoryId);
            return View(product);
        }



            [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Product/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);  // Trả về view Delete.cshtml với sản phẩm
        }

        // Action để thực hiện xóa sản phẩm
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Product/DeleteConfirmed/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteAsync(id);  // Xóa sản phẩm khỏi cơ sở dữ liệu
            return RedirectToAction(nameof(Index));    // Quay lại trang danh sách sản phẩm
        }



        [HttpGet]
        public async Task<IActionResult> Indexs(string searchTerm)
        {
            var products = await _productRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            ViewBag.SearchTerm = searchTerm;
            return View("Index", products);
        }




    }
}
