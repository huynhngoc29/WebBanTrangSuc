using WebBanTrangSuc.Models;

public interface IShoppingCartRepository
{
    Task<ShoppingCart> GetCartByUserIdAsync(string userId);
    Task AddToCartAsync(string userId, int productId, int quantity);
    Task SaveCartAsync(ShoppingCart cart);
}
