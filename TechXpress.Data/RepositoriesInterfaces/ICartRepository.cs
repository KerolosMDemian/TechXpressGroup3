using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Entities;

namespace TechXpress.Data.RepositoriesInterfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetUserCartWithItemsAsync(string userId);
    }
}
