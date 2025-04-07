using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanTrangSuc.Areas.Admin.Models;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;
using X.PagedList;
using X.PagedList.Extensions;

namespace WebBanTrangSuc.Controllers
{
    //[Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        // Hiển thị form thêm sản phẩm mới
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
 
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                await _productRepository.AddAsync(product);
                // Tự tạo variant nếu là nhẫn
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                if (category != null && category.Name.ToLower() == "nhẫn")
                {
                    var defaultSizes = new List<string> { "45", "48", "50", "52", "55" };

                    foreach (var size in defaultSizes)
                    {
                        _context.ProductVariants.Add(new ProductVariant
                        {
                            ProductId = product.Id,
                            Size = size,
                            Stock = 0
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Product", "Admin", new { area = "Admin" }); // Sau khi thêm thành công, chuyển hướng về trang danh sách sản phẩm
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        [Authorize(Roles = SD.Role_Admin)]
        private async Task<string> SaveImage(IFormFile image)
        {
            //Thay đổi đường dẫn theo cấu hình của bạn
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        //Nhớ tạo folder images trong wwwroot
        //public async Task<IActionResult> Display(int id)
        //{
        //    var product = await _productRepository.GetByIdAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    // Lấy tất cả sản phẩm và lọc sản phẩm tương tự theo CategoryId
        //    var allProducts = await _productRepository.GetAllAsync(); // Lấy tất cả sản phẩm
        //    var similarProducts = allProducts
        //        .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
        //        .Take(4) // Lấy tối đa 4 sản phẩm tương tự
        //        .ToList();

        //    // Truyền dữ liệu vào View
        //    ViewBag.SimilarProducts = similarProducts;
        //    return View(product);
        //}

        public async Task<IActionResult> Display(int id)
        {
            // Gọi phương thức mới để lấy sản phẩm kèm Category & Variants
            var product = await _productRepository.GetByIdWithCategoryAndVariantsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy các sản phẩm tương tự (cùng Category, khác ID hiện tại)
            var allProducts = await _productRepository.GetAllAsync();
            var similarProducts = allProducts
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
                .ToList();

            ViewBag.SimilarProducts = similarProducts;

            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int id)
        {
            // Lấy sản phẩm kèm Variants và Category
            var product = await _productRepository.GetByIdWithCategoryAndVariantsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }


        // Xử lý cập nhật sản phẩm
        [HttpPost]

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
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction("Product", "Admin", new { area = "Admin" }); // Quay lại danh sách sản phẩm
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form và các danh mục
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }


        // Hiển thị form xác nhận xóa sản phẩm
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction("Product", "Admin", new { area = "Admin" });
        }

        [HttpGet]

        //public async Task<IActionResult> Index(string searchTerm, int? categoryId, int page = 1, int pageSize = 12)
        //{
        //    var products = await _productRepository.GetAllAsync();

        //    // Lọc theo tên
        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        products = products
        //            .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        //            .ToList();
        //    }

        //    // Lọc theo danh mục
        //    if (categoryId.HasValue && categoryId > 0)
        //    {
        //        products = products.Where(p => p.CategoryId == categoryId).ToList();
        //    }

        //    // 👉 Sắp xếp theo ngày tạo mới nhất
        //    products = products.OrderByDescending(p => p.CreatedAt).ToList();

        //    // Truyền dữ liệu phân trang
        //    var pagedProducts = products.ToPagedList(page, pageSize);

        //    ViewBag.SearchTerm = searchTerm;
        //    ViewBag.SelectedCategory = categoryId;

        //    return View(pagedProducts);
        //}
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, string sortBy, int page = 1, int pageSize = 12)
        {
            var products = await _productRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products
                    .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
            }

            // 👉 Xử lý sắp xếp
            switch (sortBy)
            {
                case "moi-nhat":
                    products = products.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
                case "gia-thap-den-cao":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "gia-cao-den-thap":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "ban-chay":
                    products = products.OrderByDescending(p => p.QuantitySold).ToList(); // cần field QuantitySold
                    break;
                default:
                    break; // liên quan mặc định
            }

            var pagedProducts = products.ToPagedList(page, pageSize);

            // Truyền dữ liệu sang view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SortBy = sortBy;

            return View(pagedProducts);
        }





        public async Task<IActionResult> FlashSale()
        {
            var products = await _productRepository.GetAllAsync();
            var flashSaleProducts = products
                .Where(p => p.IsOnSale && p.DiscountPercentage > 0)
                .OrderByDescending(p => p.DiscountPercentage)
                .ToList();

            return View(flashSaleProducts);
        }
       

        public async Task<IActionResult> NewProducts(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 8;

            var products = await _productRepository.GetAllAsync();
            var newProducts = products
                .OrderByDescending(p => p.CreatedAt)
                .ToPagedList(pageNumber, pageSize);

            return View(newProducts);
        }



    }
}