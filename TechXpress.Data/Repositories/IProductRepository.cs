using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress.Data.Entities;

namespace TechXpress.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}
