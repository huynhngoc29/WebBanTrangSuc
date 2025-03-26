namespace WebBanTrangSuc.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }  // Thêm khóa chính cho ShoppingCart
        public string UserId { get; set; }  // Id của người dùng
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();  // Danh sách các mặt hàng trong giỏ hàng
    }
}
