using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm Flash Sale (IsOnSale = true, DiscountPercentage > 0)
            var flashSaleProducts = _context.Products
                .Where(p => p.IsOnSale && p.DiscountPercentage > 0)
                .OrderByDescending(p => p.DiscountPercentage)
                .Take(4)
                .ToList();

            // Lấy danh sách sản phẩm mới (mới nhất theo Id)
            var newProducts = _context.Products
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToList();

            var viewModel = new HomeViewModel
            {
                FlashSaleProducts = flashSaleProducts,
                NewProducts = newProducts
            };

            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
