//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using WebsiteBanHang.Areas.Admin.Models;
//using WebsiteBanHang.Models;
//using WebsiteBanHang.Repositories;

//namespace WebsiteBanHang.Areas.Admin.Controllers
//{
//    //[Area("Admin")]
//    //[Authorize(Roles = SD.Role_Admin)]
//    public class ProductController : Controller
//    {
//        private readonly IProductRepository _productRepository;
//        private readonly ICategoryRepository _categoryRepository;


//        public ProductController(IProductRepository productRepository,
//        ICategoryRepository categoryRepository)
//        {
//            _productRepository = productRepository;
//            _categoryRepository = categoryRepository;
//        }
//        // Hiển thị danh sách sản phẩm
//        public async Task<IActionResult> Index()
//        {
//            var products = await _productRepository.GetAllAsync();
//            return View(products);
//        }
//        //public async Task<IActionResult> Index(string search)
//        //{
//        //    var products = await _productRepository.GetAllAsync();

//        //    if (!string.IsNullOrEmpty(search))
//        //    {
//        //        products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
//        //    }

//        //    return View(products);
//        //}


//        //public async Task<IActionResult> Index(string search, string sortOrder)
//        //{
//        //    var products = await _productRepository.GetAllAsync();

//        //    // Tìm kiếm sản phẩm theo tên
//        //    if (!string.IsNullOrEmpty(search))
//        //    {
//        //        products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
//        //    }

//        //    // Xử lý sắp xếp theo giá
//        //    ViewBag.SortOrder = sortOrder; // Lưu giá trị sắp xếp vào ViewBag để hiển thị trên UI
//        //    switch (sortOrder)
//        //    {
//        //        case "asc":
//        //            products = products.OrderBy(p => p.Price);
//        //            break;
//        //        case "desc":
//        //            products = products.OrderByDescending(p => p.Price);
//        //            break;
//        //    }

//        //    return View(products);
//        //}
//        //public async Task<IActionResult> Index(string search, string sortOrder, int? categoryId)
//        //{
//        //    var products = await _productRepository.GetAllAsync();

//        //    // Lọc theo từ khóa tìm kiếm
//        //    if (!string.IsNullOrEmpty(search))
//        //    {
//        //        products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
//        //    }
//        //    // Lọc theo danh mục
//        //    if (categoryId.HasValue && categoryId > 0)
//        //    {
//        //        products = products.Where(p => p.CategoryId == categoryId);
//        //    }
//        //    // Lưu giá trị tìm kiếm vào ViewBag để giữ lại khi sắp xếp
//        //    ViewBag.Search = search;
//        //    ViewBag.SortOrder = sortOrder;

//        //    // Xử lý sắp xếp theo giá
//        //    switch (sortOrder)
//        //    {
//        //        case "asc":
//        //            products = products.OrderBy(p => p.Price);
//        //            break;
//        //        case "desc":
//        //            products = products.OrderByDescending(p => p.Price);
//        //            break;
//        //    }

//        //    return View(products);
//        //}

//        public async Task<IActionResult> Index(string search, string sortOrder, int? categoryId)
//        {
//            var products = await _productRepository.GetAllAsync();

//            // Lấy danh sách danh mục từ repository
//            var categories = await _categoryRepository.GetAllAsync();
//            ViewBag.Categories = new SelectList(categories, "Id", "Name");  // ✅ Khởi tạo ViewBag.Categories

//            // Lọc theo từ khóa tìm kiếm
//            if (!string.IsNullOrEmpty(search))
//            {
//                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
//            }

//            // Lọc theo danh mục (categoryId)
//            if (categoryId.HasValue && categoryId > 0)
//            {
//                products = products.Where(p => p.CategoryId == categoryId);
//            }

//            // Lưu thông tin bộ lọc vào ViewBag để duy trì trạng thái trên giao diện
//            ViewBag.Search = search;
//            ViewBag.SortOrder = sortOrder;
//            ViewBag.CategoryId = categoryId;

//            // Xử lý sắp xếp theo giá
//            switch (sortOrder)
//            {
//                case "asc":
//                    products = products.OrderBy(p => p.Price);
//                    break;
//                case "desc":
//                    products = products.OrderByDescending(p => p.Price);
//                    break;
//            }

//            return View(products);
//        }


//        // Hiển thị form thêm sản phẩm mới
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Add()
//        {
//            var categories = await _categoryRepository.GetAllAsync();
//            ViewBag.Categories = new SelectList(categories, "Id", "Name");
//            return View();
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
//        {
//            if (ModelState.IsValid)
//            {
//                if (imageUrl != null)
//                {
//                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
//                    product.ImageUrl = await SaveImage(imageUrl);
//                }
//                await _productRepository.AddAsync(product);
//                return RedirectToAction(nameof(Index));
//            }
//            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
//            var categories = await _categoryRepository.GetAllAsync();
//            ViewBag.Categories = new SelectList(categories, "Id", "Name");
//            return View(product);
//        }

//        private async Task<string> SaveImage(IFormFile image)
//        {
//            //Thay đổi đường dẫn theo cấu hình của bạn
//            var savePath = Path.Combine("wwwroot/images", image.FileName);
//            using (var fileStream = new FileStream(savePath, FileMode.Create))
//            {
//                await image.CopyToAsync(fileStream);
//            }
//            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
//        }
//        //Nhớ tạo folder images trong wwwroot
//        public async Task<IActionResult> Display(int id)
//        {
//            var product = await _productRepository.GetByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }
//            return View(product);
//        }
//        // Hiển thị form cập nhật sản phẩm
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Update(int id)
//        {
//            // Lấy sản phẩm theo id
//            var product = await _productRepository.GetByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }

//            // Kiểm tra sự tồn tại của CategoryId trong bảng Categories
//            var categoryExists = await _categoryRepository.GetByIdAsync(product.CategoryId);
//            if (categoryExists == null)
//            {
//                ModelState.AddModelError("CategoryId", "Danh mục không tồn tại.");
//                return View(product); // Trả lại view nếu không tìm thấy danh mục
//            }

//            // Lấy tất cả danh mục và truyền vào ViewBag
//            var categories = await _categoryRepository.GetAllAsync();
//            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
//            return View(product); // Trả lại view khi thông tin hợp lệ
//        }

//        // Xử lý cập nhật sản phẩm
//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
//        {
//            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho trường ImageUrl

//            if (id != product.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                var existingProduct = await _productRepository.GetByIdAsync(id); // Lấy sản phẩm hiện tại từ DB

//                // Kiểm tra nếu không có ảnh mới, giữ nguyên ảnh cũ
//                if (imageUrl == null)
//                {
//                    product.ImageUrl = existingProduct.ImageUrl;
//                }
//                else
//                {
//                    // Lưu ảnh mới nếu có
//                    product.ImageUrl = await SaveImage(imageUrl);
//                }

//                existingProduct.Name = product.Name;
//                existingProduct.Price = product.Price;
//                existingProduct.Description = product.Description;
//                existingProduct.CategoryId = product.CategoryId;
//                existingProduct.ImageUrl = product.ImageUrl;

//                await _productRepository.UpdateAsync(existingProduct);
//                return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
//            }

//            // Nếu ModelState không hợp lệ, hiển thị lại form và các danh mục
//            var categories = await _categoryRepository.GetAllAsync();
//            ViewBag.Categories = new SelectList(categories, "Id", "Name");
//            return View(product);
//        }


//        // Hiển thị form xác nhận xóa sản phẩm
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var product = await _productRepository.GetByIdAsync(id);
//            if (product == null)
//            {
//                return NotFound();
//            }
//            return View(product);
//        }
//        // Xử lý xóa sản phẩm
//        [Authorize(Roles = "Admin")]
//        [HttpPost, ActionName("DeleteConfirmed")]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _productRepository.DeleteAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//        [Authorize(Roles = "Admin")]
//        public IActionResult Create()
//        {
//            return View();
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Create(Product product)
//        {
//            if (ModelState.IsValid)
//            {
//                await _productRepository.AddAsync(product);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(product);

//        }


//    }
//}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanTrangSuc.Areas.Admin.Models;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;

namespace WebBanTrangSuc.Areas.Admin.Controllers
{
    //[Area("Admin")]
    //[Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập các hành động này
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;


        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null && quantity > 0)
            {
                // Thêm sản phẩm vào giỏ hàng
                await _shoppingCartRepository.AddToCartAsync(User.Identity.Name, productId, quantity);

                // Trả về thông tin sản phẩm để hiển thị trong modal
                return Json(new
                {
                    productName = product.Name,
                    price = product.Price
                });
            }

            return Json(new { success = false });
        }

        // Hiển thị danh sách sản phẩm cho Admin
        public async Task<IActionResult> Index(string search, string sortOrder, int? categoryId)
        {
            var products = await _productRepository.GetAllAsync();

            // Lấy danh sách danh mục từ repository
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo danh mục (categoryId)
            if (categoryId.HasValue && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            // Lưu thông tin bộ lọc vào ViewBag để duy trì trạng thái trên giao diện
            ViewBag.Search = search;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CategoryId = categoryId;

            // Xử lý sắp xếp theo giá
            switch (sortOrder)
            {
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
            }

            return View(products);
        }

        // Hiển thị form thêm sản phẩm mới cho Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm mới
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm cho Admin
        [Authorize(Roles = "Admin")]
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
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

      
        // Hiển thị form xác nhận xóa sản phẩm
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm khi người dùng xác nhận
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm
        }


        // Hiển thị danh sách sản phẩm cho Người dùng (User)
        public async Task<IActionResult> UserView()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // Lọc sản phẩm theo danh mục cho Người dùng
        public async Task<IActionResult> FilterByCategory(int? categoryId)
        {
            var products = await _productRepository.GetAllAsync();
            if (categoryId.HasValue && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }
            return View("UserView", products);  // Trả về view danh sách sản phẩm
        }
    }
}
