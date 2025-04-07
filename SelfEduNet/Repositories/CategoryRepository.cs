using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetAsync(Func<Category, bool> predicate);
		Task<bool> AddAsync(Category category);
        Task<bool> SaveAsync();
    }
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        private readonly ApplicationDbContext _context = context;

		public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
	            .Where(c => c.Title !="Drafts")
	            .ToListAsync();
        }
		public async Task<Category> GetAsync(Func<Category, bool> predicate)
		{
			// Use AsQueryable to enable LINQ queries and AsNoTracking for read-only queries
			return await Task.Run(() => _context.Categories.AsQueryable().FirstOrDefault(predicate));
		}
		public async Task<bool> AddAsync(Category category)
        {
	        await _context.Categories.AddAsync(category);
	        return await SaveAsync();
        }
        public async Task<bool> SaveAsync()
        {
	        return await _context.SaveChangesAsync() > 0;
        }
	}
}
