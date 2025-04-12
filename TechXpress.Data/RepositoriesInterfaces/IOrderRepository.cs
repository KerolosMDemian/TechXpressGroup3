using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.Entities;

namespace TechXpress.Data.RepositoriesInterfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order> GetOrderWithDetailsAsync(int orderId);

    }
}
