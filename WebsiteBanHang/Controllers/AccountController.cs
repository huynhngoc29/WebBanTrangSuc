using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanTrangSuc.Models;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Manage Profile
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        // Lấy vai trò của người dùng
        var roles = await _userManager.GetRolesAsync(user);

        // Truyền vai trò vào ViewBag để sử dụng trong View
        ViewBag.Roles = roles;

        return View(user); // Trả về trang Profile với thông tin người dùng và vai trò
    }
}
