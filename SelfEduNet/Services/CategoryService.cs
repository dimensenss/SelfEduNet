using Microsoft.EntityFrameworkCore;
using SelfEduNet.Models;
using SelfEduNet.Repositories;

namespace SelfEduNet.Services
{
	public interface ICategoryService
	{
		Task<Category> GetOrCreateDraftsCategoryAsync();
		Task<Category?> GetCategoryByIdAsync(int id);
		Task<Category?> GetCategoryByTitleAsync(string title);
		Task<IEnumerable<Category>> GetAllCategoriesAsync();
	}

	public class CategoryService(ICategoryRepository categoryRepository):ICategoryService
	{
		private readonly ICategoryRepository _categoryRepository = categoryRepository;

		public async Task<Category> GetOrCreateDraftsCategoryAsync()
		{
			var draftsCategory = await _categoryRepository.GetAsync(c => c.Title == "Drafts");

			if (draftsCategory == null)
			{
				draftsCategory = new Category { Title = "Drafts" };
				await _categoryRepository.AddAsync(draftsCategory);
			}

			return draftsCategory;
		}

		public async Task<Category?> GetCategoryByIdAsync(int id)
		{
			return await _categoryRepository.GetCategoryByIdAsync(id);
		}

		public async Task<Category?> GetCategoryByTitleAsync(string title)
		{
			return await _categoryRepository.GetCategoryByTitleAsync(title);
			
		}

		public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
		{
			return await _categoryRepository.GetAllCategoriesAsync();
		}
	}
}
