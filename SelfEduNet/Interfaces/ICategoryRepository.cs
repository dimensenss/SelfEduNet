using SelfEduNet.Models;

namespace SelfEduNet.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

    }
}
