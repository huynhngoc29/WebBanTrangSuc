using System.Collections.Generic;
using System.Threading.Tasks;
using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Repositories
{
    public interface ISubCategoryRepository
    {
        Task<IEnumerable<SubCategory>> GetAllAsync();
        Task<SubCategory?> GetByIdAsync(int id);
        Task AddAsync(SubCategory subCategory);
        Task UpdateAsync(SubCategory subCategory);
        Task DeleteAsync(int id);
    }
}
