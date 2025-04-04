using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Repositories
{
    public interface IProductRepository
    {
        //IEnumerable<Product> GetAll();
        //Product GetById(int id);
        //void Add(Product product);
        //void Update(Product product);
        //void Delete(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> GetFlashSaleProductsAsync();

    }
}
