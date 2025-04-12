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
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
       

        public CartRepository(AppDbContext context) : base(context)
        {
            
        }

       public async Task<Cart?> GetUserCartWithItemsAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        
    }
}
