using Microsoft.EntityFrameworkCore;
using WebBanTrangSuc.Models;

public class EFShoppingCartRepository : IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;

    public EFShoppingCartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy giỏ hàng của người dùng
    public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
    {
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product) // Bao gồm sản phẩm trong giỏ hàng
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new ShoppingCart { UserId = userId };
            _context.ShoppingCarts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    // Thêm sản phẩm vào giỏ hàng
    public async Task AddToCartAsync(string userId, int productId, int quantity)
    {
        var cart = await GetCartByUserIdAsync(userId);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            // Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
            existingItem.Quantity += quantity;
        }
        else
        {
            // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                cart.Items.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    Product = product,
                    Quantity = quantity
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    // Lưu giỏ hàng (chẳng hạn nếu có thay đổi)
    public async Task SaveCartAsync(ShoppingCart cart)
    {
        _context.ShoppingCarts.Update(cart);
        await _context.SaveChangesAsync();
    }
    public void RemoveFromCart(int productId)
    {
        var cartItem = _context.ShoppingCartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem != null)
        {
            _context.ShoppingCartItems.Remove(cartItem);
            _context.SaveChanges();
        }
    }
}
