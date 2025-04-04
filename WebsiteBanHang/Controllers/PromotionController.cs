using Microsoft.AspNetCore.Mvc;
using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var promotions = _context.Promotions.ToList();
            return View(promotions);
        }
    }
}
