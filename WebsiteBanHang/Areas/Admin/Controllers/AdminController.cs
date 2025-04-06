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

namespace WebBanTrangSuc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    [Route("Admin/")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AdminController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Route cho trang chủ
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        // Route cho danh sách sản phẩm
        [Route("Product")]
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
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var validCategories = categories.Where(c => c.Name != null).ToList(); // Lọc bỏ danh mục NULL
            ViewBag.Categories = new SelectList(validCategories, "Id", "Name"); // Chuyển danh sách sang SelectList

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
                return RedirectToAction(nameof(Index));
            }

            // Nếu không thành công, hiển thị lại form với thông tin đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
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
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categoryExists = await _categoryRepository.GetByIdAsync(product.CategoryId);
            if (categoryExists == null)
            {
                ModelState.AddModelError("CategoryId", "Danh mục không tồn tại.");
                return View(product);
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }



        // Xử lý cập nhật sản phẩm
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Product/Update/{id}")]

        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl");

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.Quantity = product.Quantity;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }


        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //[Route("Product/Delete/{id}")]

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var product = await _productRepository.GetByIdAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //// Xử lý xóa sản phẩm khi người dùng xác nhận
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("DeleteConfirmed")]
        //[Route("Product/DeleteConfirmed/{id}")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    await _productRepository.DeleteAsync(id);
        //    return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
        //}// Action để hiển thị trang xác nhận xóa
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
