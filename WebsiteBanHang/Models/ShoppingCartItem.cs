using WebBanTrangSuc.Models;

public class ShoppingCartItem
{
    public int Id { get; set; }  // Khóa chính cho ShoppingCartItem
    public int ProductId { get; set; }  // Sản phẩm
    public Product Product { get; set; }  // Thông tin sản phẩm
    public int Quantity { get; set; }  // Số lượng
}