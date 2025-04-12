using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;

namespace TechXpress.Data.Repositories
{
    public class ProductRepository<T> : IProductRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task Add(T entity)
        {
          await  _dbSet.AddAsync(entity);
            
        }

        public void Delete(T entity)
        {
             _dbSet.Remove(entity);
            _context.SaveChanges(); 
        }

        public IEnumerable<T> GetAllProduct()
        {
            return _dbSet.ToList();
        }

        public  T GetProductById(int id)
        {
            var product = _dbSet.Find(id);
            if (product == null)
            {
                throw new Exception($"Product with ID {id} not found.");
            }
            return product;
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges(); 
        }
    }
}
