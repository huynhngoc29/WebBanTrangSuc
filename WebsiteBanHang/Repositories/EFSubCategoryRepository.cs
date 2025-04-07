using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Repositories
{
    public class EFSubCategoryRepository : ISubCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFSubCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubCategory>> GetAllAsync()
        {
            return await _context.SubCategories.ToListAsync();
        }

        public async Task<SubCategory?> GetByIdAsync(int id)
        {
            return await _context.SubCategories.FindAsync(id);
        }

        public async Task AddAsync(SubCategory subCategory)
        {
            await _context.SubCategories.AddAsync(subCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SubCategory subCategory)
        {
            _context.SubCategories.Update(subCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory != null)
            {
                _context.SubCategories.Remove(subCategory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
