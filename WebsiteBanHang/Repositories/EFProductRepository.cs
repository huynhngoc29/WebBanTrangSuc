﻿using Microsoft.EntityFrameworkCore;
using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
        // Repository: EFProductRepository
        public async Task<IEnumerable<Product>> GetFlashSaleProductsAsync()
        {
            // Lọc các sản phẩm có giảm giá (IsOnSale = true) và DiscountPercentage > 0
            return await _context.Products
                .Where(p => p.IsOnSale && p.DiscountPercentage > 0)
                .ToListAsync();
        }


        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<Product?> GetByIdWithCategoryAndVariantsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Product?> GetByIdWithVariantsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.Category) // nếu cần cho hiển thị tên danh mục
                .FirstOrDefaultAsync(p => p.Id == id);
        }


    }
}
