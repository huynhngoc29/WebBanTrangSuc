using Microsoft.AspNetCore.Mvc;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;

namespace WebBanTrangSuc.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            await _shoppingCartRepository.AddToCartAsync(User.Identity.Name, productId, quantity);
            return RedirectToAction("Index", "ShoppingCart");
        }

        // Xem giỏ hàng
        public async Task<IActionResult> Index()
        {
            var cart = await _shoppingCartRepository.GetCartByUserIdAsync(User.Identity.Name);
            return View(cart);
        }

    }
}
