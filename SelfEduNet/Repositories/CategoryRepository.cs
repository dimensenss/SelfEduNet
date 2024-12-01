﻿using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Interfaces;
using SelfEduNet.Models;

namespace SelfEduNet.Repositories
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
