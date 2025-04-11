using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress.Data.Entities;

namespace TechXpress.Data.RepositoriesInterfaces
{
    public interface IProductRepository<T> where T : class

    {
        IEnumerable<T> GetAllProduct();
        T GetProductById(int id);
        public Task Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public Task Save();


    }
}
