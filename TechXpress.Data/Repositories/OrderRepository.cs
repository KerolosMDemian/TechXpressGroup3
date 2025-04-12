using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.RepositoriesInterfaces;

namespace TechXpress.Data.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        
        public OrderRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .ThenInclude(u => u.Address)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        
    }
}
