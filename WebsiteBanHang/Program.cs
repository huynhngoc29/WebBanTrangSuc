//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using WebsiteBanHang.Models;
//using WebsiteBanHang.Repositories;

//var builder = WebApplication.CreateBuilder(args);

//// Đảm bảo thêm DbContext với đúng chuỗi kết nối từ appsettings.json
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddDefaultTokenProviders()
//    .AddDefaultUI()
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//// Thêm dịch vụ RazorPages
//builder.Services.AddRazorPages();

//builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IProductRepository, EFProductRepository>();
//builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = $"/Identity/Account/Login";
//    options.LogoutPath = $"/Identity/Account/Logout";
//    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
//});
//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//}
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "areas",
//        pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}"
//    );
//    endpoints.MapControllerRoute(
//       name: "category",
//       pattern: "Category/{action=Index}/{id?}",
//       defaults: new { controller = "Category" }
//   );
//});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//// Định tuyến Razor Pages
//app.MapRazorPages();

//app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanTrangSuc.Models;
using WebBanTrangSuc.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Đảm bảo thêm DbContext với đúng chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity cho ứng dụng
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Thêm dịch vụ RazorPages
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

// Đăng ký các repository
builder.Services.AddScoped<IShoppingCartRepository, EFShoppingCartRepository>(); // Đảm bảo bạn sử dụng đúng tên repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

// Cấu hình đường dẫn cho Cookie khi đăng nhập
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Định tuyến các Controller và Area
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "category",
        pattern: "Category/{action=Index}/{id?}",
        defaults: new { controller = "Category" }
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Định tuyến Razor Pages
app.MapRazorPages();

app.Run();
