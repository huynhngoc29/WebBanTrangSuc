using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanTrangSuc.Extensions;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;

namespace WebBanTrangSuc.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
   
        public ShoppingCartController(ApplicationDbContext context,UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }
        //public async Task<IActionResult> AddToCart(int productId, int quantity)
        //{
        //    // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId 
        //    var product = await GetProductFromDatabase(productId);

        //    var cartItem = new CartItem
        //    {
        //        ProductId = productId,
        //        Name = product.Name,
        //        Price = product.Price,
        //        Quantity = quantity
        //    };
        //    var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ??
        //new ShoppingCart();
        //    cart.AddItem(cartItem);

        //    HttpContext.Session.SetObjectAsJson("Cart", cart);

        //    return RedirectToAction("Index");
        //}
        public async Task<IActionResult> AddToCart(int productId, int quantity, string? size = null)
        {
            var product = await GetProductFromDatabase(productId);

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
            };

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ??
           new ShoppingCart();
            return View(cart);
        }
        // Các actions khác... 
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm 
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart is not null)
            {
                cart.RemoveItem(productId);

                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục 
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Nếu giỏ hàng trống thì quay lại
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);

            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

            order.OrderDetails = new List<OrderDetail>(); // Khởi tạo danh sách

            //foreach (var item in cart.Items)
            //{
            //    // Trừ số lượng tồn kho
            //    var product = await _context.Products.FindAsync(item.ProductId);
            //    if (product != null)
            //    {
            //        product.Quantity -= item.Quantity;
            //    }

            //    order.OrderDetails.Add(new OrderDetail
            //    {
            //        ProductId = item.ProductId,
            //        Quantity = item.Quantity,
            //        Price = item.Price
            //    });
            //}
            foreach (var item in cart.Items)
            {
                // Trừ số lượng tồn kho và cộng QuantitySold
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;

                    // ✅ Cập nhật số lượng đã bán
                    product.QuantitySold += item.Quantity;
                }

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                });
            }


            // ✅ Save đơn hàng
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Xoá giỏ hàng sau khi đặt hàng thành công
            HttpContext.Session.Remove("Cart");

            return View("OrderCompleted", order.Id);
        }

    }
}
